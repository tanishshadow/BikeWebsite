using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Foolproof
{
    [AttributeUsage(AttributeTargets.Property)]
    public abstract class ModelAwareValidationAttribute : ValidationAttribute
    {
        public ModelAwareValidationAttribute() { }
        
        static ModelAwareValidationAttribute()
        {
            Register.All();            
        }    

        public override bool IsValid(object value)
        {
            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            if (string.IsNullOrEmpty(ErrorMessageResourceName) && string.IsNullOrEmpty(ErrorMessage))
                ErrorMessage = DefaultErrorMessage;
            
            return base.FormatErrorMessage(name);
        }

        public virtual string DefaultErrorMessage
        {
            get { return "{0} is invalid."; }
        }

        public abstract bool IsValid(object value, object container);
		
		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			var container = validationContext.ObjectInstance;

			#region This block of code decompiled from namespace System.ComponentModel.DataAnnotations.ValidationAttribute
			ValidationResult local_0 = ValidationResult.Success;
			if (!this.IsValid(value, container)) // Change to decompiled code here to call our abstract implementation instead of the NotImplemented IsValid(object value) method above
			{
				string[] temp_26;
				if (validationContext.MemberName == null)
					temp_26 = (string[])null;
				else
					temp_26 = new string[1]
					{
						validationContext.MemberName
					};
				string[] local_1 = temp_26;
				local_0 = new ValidationResult(this.FormatErrorMessage(validationContext.DisplayName), (IEnumerable<string>)local_1);
			}
			return local_0;
			#endregion
		}

        public virtual string ClientTypeName
        {
            get { return this.GetType().Name.Replace("Attribute", ""); }
        }

        protected virtual IEnumerable<KeyValuePair<string, object>> GetClientValidationParameters()
        {
            return new KeyValuePair<string, object>[0];
        }
        
        public Dictionary<string, object> ClientValidationParameters
        {
            get { return GetClientValidationParameters().ToDictionary(kv => kv.Key.ToLower(), kv => kv.Value); }
        }
    }
}
