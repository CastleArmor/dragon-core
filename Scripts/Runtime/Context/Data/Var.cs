using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Dragon.Core
{
    public interface IVar
    {
        Type ValueType { get; }
        object BoxedValue { get; set; }
    }

    public interface IVar<TValue> : IVar
    {
        TValue Value { get; set; }
        event Action<IContext, TValue, TValue> onValueChanged;
        void InitializeVariable(IContext context, string key);
    }
    
    [System.Serializable]
    public class Var<TValue> : IVar<TValue>
    {
        private IContext _context;
        private string _key;
        public void InitializeVariable(IContext context,string key)
        {
            _context = context;
            _key = key;
            _value = DataRegistry<TValue>.GetData(context, key);
            DataRegistry<TValue>.RegisterOnChange(context,OnDataChanged);
            DataRegistry<TValue>.RegisterVariable(context, key, this);
            if (_context != null)
            {
                _context.onDestroyContext += OnDestroyContext;
            }
        }

        private void OnDestroyContext(IContext obj)
        {
            FinalizeVariable();
        }

        private void OnDataChanged(DataOnChangeArgs<TValue> obj)
        {
            _value = obj.NewValue;
            onValueChanged?.Invoke(_context,obj.OldValue,obj.NewValue);
        }

        public void FinalizeVariable()
        {
            if (_context != null)
            {
                _context.onDestroyContext -= OnDestroyContext;
            }
            DataRegistry<TValue>.UnregisterOnChange(_context,OnDataChanged);
            DataRegistry<TValue>.UnregisterVariable(_context, _key, this);
        }
        
        [SerializeField][HideInPlayMode]
        private TValue _value;
        public event Action<IContext, TValue, TValue> onValueChanged;
        [ShowInInspector][HideInEditorMode]
        public TValue Value
        {
            get => _value;
            set => HandleSetOperation(value);
        }

        protected virtual void HandleSetOperation(TValue newValue)
        {
            DefaultSetOperation(newValue);
        }

        protected void DefaultSetOperation(TValue newValue)
        {
            DataRegistry<TValue>.SetData(_context,newValue,_key);
        }

        public Type ValueType => typeof(TValue);

        public object BoxedValue
        {
            get => Value;
            set => Value = (TValue)value;
        }
    }
}