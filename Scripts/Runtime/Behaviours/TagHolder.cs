using UnityEngine;

namespace Dragon.Core
{
    public class TagHolder : MonoBehaviour
    {
        [SerializeField] private Key _category;
        public string Category => _category?_category.ID:"Global";
    
        [SerializeField] private Key _tag;
        public string Tag => _tag.ID;

        private void OnEnable()
        {
            TagRegistry.Register(this);
        }

        private void OnDisable()
        {
            TagRegistry.Unregister(this);
        }
    }
}