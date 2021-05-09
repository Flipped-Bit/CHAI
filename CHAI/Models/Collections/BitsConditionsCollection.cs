using CHAI.Extensions;
using CHAI.Models.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CHAI.Models.Collections
{
    /// <summary>
    /// An <see cref="ObservableCollection{BitsCondition}"/>.
    /// </summary>
    public class BitsConditionsCollection : ObservableCollection<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BitsConditionsCollection"/> class.
        /// </summary>
        public BitsConditionsCollection()
        {
            foreach (var bitCondition in Enum.GetValues(typeof(BitsCondition)).Cast<BitsCondition>())
            {
                Add(bitCondition.GetDescription());
            }
        }
    }
}
