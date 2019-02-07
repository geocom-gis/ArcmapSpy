// Copyright © 2018 geocom.ch
// This program is subject to the terms of the GPL-3.0 License.
// You should have received a copy of the GNU General Public License
// along with this program. If not, see http://www.gnu.org/licenses/.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GalaSoft.MvvmLight;

namespace ArcmapSpy.ViewModels
{
    /// <summary>
    /// A base class for the ViewModel classes in the MVVM pattern.
    /// </summary>
    public class BaseViewModel : GalaSoft.MvvmLight.ViewModelBase, INotifyPropertyChanged
    {
        /// <summary>
        /// Assigns a new value to the property and raises the PropertyChanged event if necessary.
        /// </summary>
        /// <typeparam name="T">The type of the property to change.</typeparam>
        /// <param name="viewModelField">The backing field in the viewmodel.</param>
        /// <param name="newValue">The new value of the property.</param>
        /// <param name="propertyName">(optional) The name of the property to change.</param>
        /// <returns>Returns true if the PropertyChanged event was raised, otherwise false.</returns>
        protected bool ChangeProperty<T>(ref T viewModelField, T newValue, [CallerMemberName] string propertyName = null)
        {
            return Set<T>(ref viewModelField, newValue, propertyName);
        }

        /// <summary>
        /// Assigns a new value to the property and raises the PropertyChanged event if necessary.
        /// </summary>
        /// <typeparam name="T">The type of the property to change.</typeparam>
        /// <param name="getterFromModel">Gets the value of the property from the model.</param>
        /// <param name="setterToModel">Sets the value of the property to the model.</param>
        /// <param name="newValue">The new value of the property.</param>
        /// <param name="propertyName">(optional) The name of the property to change.</param>
        /// <returns>Returns true if the PropertyChanged event was raised, otherwise false.</returns>
        protected bool ChangePropertyIndirect<T>(Func<T> getterFromModel, Action<T> setterToModel, T newValue, [CallerMemberName] string propertyName = null)
        {
            T oldValue = getterFromModel();
            if (!EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                setterToModel(newValue);
                RaisePropertyChanged<T>(propertyName, oldValue, newValue, false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Raises the property changed event, to inform the view of changes.
        /// Calling this method is usually not necessary, because the method <see cref="ChangeProperty{T}(ref T, T, bool, string)"/>
        /// will call it automatically.
        /// </summary>
        /// <param name="propertyName">Name of the property which has changed.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            RaisePropertyChanged(propertyName);
        }
    }
}
