using System;

[Flags]
public enum EventAddressType
{
    OnlyContext = 0,
    Context = 1<<0,
    ContextUser = 1<<1,
    ContextRelative = 1<<2,
    ContextUserRelative = 1<<3,
    Global = 1<<4,
    FromGroupFirstMember = 1<<5,
    ToAllGroupMembers = 1<<6
}