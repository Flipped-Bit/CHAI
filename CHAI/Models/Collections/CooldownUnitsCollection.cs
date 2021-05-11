using CHAI.Extensions;
using CHAI.Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CHAI.Models.Collections
{
    /// <summary>
    /// An <see cref="ObservableCollection{CooldownUnit}"/>.
    /// </summary>
    public class CooldownUnitsCollection : ObservableCollection<KeyValuePair<string, string>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CooldownUnitsCollection"/> class.
        /// </summary>
        public CooldownUnitsCollection()
        {
            foreach (var cooldownUnit in Enum.GetValues(typeof(CooldownUnit)).Cast<CooldownUnit>())
            {
                Add(new KeyValuePair<string, string>(cooldownUnit.GetDescription(), cooldownUnit.ToString()));
            }
        }
    }
}
