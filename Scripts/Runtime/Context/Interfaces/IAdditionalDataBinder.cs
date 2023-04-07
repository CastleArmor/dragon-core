namespace Dragon.Core
{
    public interface IAdditionalDataBinder : ICastable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="assignedID"></param>
        /// <param name="isBind">defines if this is a bind or unbind operation, true if bind, false if unbind.</param>
        void OnToggleBinding(IContext context, string key, string assignedID,bool isBind);
    }
}