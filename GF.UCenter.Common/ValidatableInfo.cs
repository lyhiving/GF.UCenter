namespace GF.UCenter.Common
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    // todo: 由于无法跨平台，所以暂时移除
    public abstract class ValidatableInfo
    {
        public ICollection<ValidationResult> Errors { get; } = new List<ValidationResult>();

        public virtual bool Validate()
        {
            var context = new ValidationContext(this, null, null);
            this.Errors.Clear();
            return Validator.TryValidateObject(this, context, this.Errors, true);
        }
    }
}