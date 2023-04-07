namespace Dragon.Core
{
    public static class DEventConstructs
    {
        public static IContext GetReturnEventAddressMain(IContext context,ReturnEventAddressType addressType)
        {
            switch (addressType)
            {
                case ReturnEventAddressType.Context : 
                    return context;
                default: return context;
            }
        }  
    }
}