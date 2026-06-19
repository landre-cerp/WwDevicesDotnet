// SPDX-FileCopyrightText: 2025 Andrew Whewell
// SPDX-License-Identifier: BSD-3-Clause

using System;

namespace WwDevicesDotNet
{
    public class KeyEventArgs : EventArgs
    {
        public Key Key { get; }

        public CommonKey CommonKey => Key.ToCommonKey();

        public string Character { get; }

        public bool Pressed { get; }

        public KeyEventArgs(Key key, bool pressed)
        {
            Key = key;
            Character = key.ToCharacter();
            Pressed = pressed;
        }
    }
}
