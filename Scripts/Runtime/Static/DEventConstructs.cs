public static class DEventConstructs
{
    public static IEventContext GetReturnEventAddressMain(IEventContext context,ReturnEventAddressType addressType)
    {
        switch (addressType)
        {
            case ReturnEventAddressType.Context : 
                return context;
            default: return context;
        }
    }  
}