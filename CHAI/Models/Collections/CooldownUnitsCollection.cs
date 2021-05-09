using CHAI.Extensions;
using CHAI.Models.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CHAI.Models.Collections
{
    /// <summary>
    /// An <see cref="ObservableCollection{CooldownUnit}"/>.
    /// </summary>
    public class CooldownUnitsCollection : ObservableCollection<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CooldownUnitsCollection"/> class.
        /// </summary>
        public CooldownUnitsCollection()
        {
            foreach (var cooldownUnit in Enum.GetValues(typeof(CooldownUnit)).Cast<CooldownUnit>())
            {
                Add(cooldownUnit.GetDescription());
            }
        }
    }
}
