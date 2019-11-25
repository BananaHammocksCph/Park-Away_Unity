// This source code is dual-licensed under the Apache License, version
// 2.0, and the Mozilla Public License, version 1.1.
//
// The APL v2.0:
//
//---------------------------------------------------------------------------
//   Copyright (C) 2007-2014 GoPivotal, Inc.
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//---------------------------------------------------------------------------
//
// The MPL v1.1:
//
//---------------------------------------------------------------------------
//  The contents of this file are subject to the Mozilla Public License
//  Version 1.1 (the "License"); you may not use this file except in
//  compliance with the License. You may obtain a copy of the License
//  at http://www.mozilla.org/MPL/
//
//  Software distributed under the License is distributed on an "AS IS"
//  basis, WITHOUT WARRANTY OF ANY KIND, either express or implied. See
//  the License for the specific language governing rights and
//  limitations under the License.
//
//  The Original Code is RabbitMQ.
//
//  The Initial Developer of the Original Code is GoPivotal, Inc.
//  Copyright (c) 2007-2014 GoPivotal, Inc.  All rights reserved.
//---------------------------------------------------------------------------

using System;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

// We use spec version 0-9 for common constants such as frame types,
// error codes, and the frame end byte, since they don't vary *within
// the versions we support*. Obviously we may need to revisit this if
// that ever changes.
using RabbitMQ.Client.Framing;

namespace RabbitMQ.Client.Impl
{
    ///<summary>Small ISession implementation used only for channel 0.</summary>
    public class MainSession: Session
    {
        public bool m_closing = false;
        public int m_closeClassId;
        public int m_closeMethodId;
        public int m_closeOkClassId;
        public int m_closeOkMethodId;

        public bool m_closeServerInitiated;

        private readonly object m_closingLock = new object();
        public delegate void SessionCloseDelegate();
        public SessionCloseDelegate m_handler;

        public MainSession(RabbitMQ.Client.Framing.Impl.Connection connection)
            : base(connection, 0)
        {
            Command request;
            connection.Protocol.CreateConnectionClose(0,"",
                                                      out request,
                                                      out m_closeOkClassId,
                                                      out m_closeOkMethodId);
            m_closeClassId = request.Method.ProtocolClassId;
            m_closeMethodId = request.Method.ProtocolMethodId;
        }

        ///<summary> Set channel 0 as quiescing </summary>
        ///<remarks>
        /// Method should be idempotent. Cannot use base.Close
        /// method call because that would prevent us from
        /// sending/receiving Close/CloseOk commands
        ///</remarks>
        public void SetSessionClosing(bool closeServerInitiated)
        {
            lock(m_closingLock)
            {
                if (!m_closing)
                {
                    m_closing = true;
                    m_closeServerInitiated = closeServerInitiated;
                }
            }
        }

        public SessionCloseDelegate Handler
        {
            get { return m_handler; }
            set { m_handler = value; }
        }

        public override void HandleFrame(Frame frame)
        {

            lock(m_closingLock)
            {
                if (!m_closing)
                {
                    base.HandleFrame(frame);
                    return;
                }
            }

            if (!m_closeServerInitiated
                && (frame.Type == Constants.FrameMethod))
            {
                MethodBase method = Connection.Protocol.DecodeMethodFrom(frame.GetReader());
                if ((method.ProtocolClassId == m_closeClassId)
                    && (method.ProtocolMethodId == m_closeMethodId))
                {
                    base.HandleFrame(frame);
                    return;
                }

                if ((method.ProtocolClassId == m_closeOkClassId)
                    && (method.ProtocolMethodId == m_closeOkMethodId))
                {
                    // This is the reply (CloseOk) we were looking for
                    // Call any listener attached to this session
                    this.Handler();
                    return;
                }
            }

            // Either a non-method frame, or not what we were looking
            // for. Ignore it - we're quiescing.
        }


        public override void Transmit(Command cmd)
        {
            lock(m_closingLock)
            {
                if (!m_closing)
                {
                    base.Transmit(cmd);
                    return;
                }
            }

            // Allow always for sending close ok
            // Or if application initiated, allow also for sending close
            MethodBase method = cmd.m_method;
            if ( ((method.ProtocolClassId == m_closeOkClassId)
                  && (method.ProtocolMethodId == m_closeOkMethodId))
                  || (!m_closeServerInitiated && (
                      (method.ProtocolClassId == m_closeClassId) &&
                      (method.ProtocolMethodId == m_closeMethodId))
                      ))
            {
                base.Transmit(cmd);
                return;
            }
        }
    }
}
