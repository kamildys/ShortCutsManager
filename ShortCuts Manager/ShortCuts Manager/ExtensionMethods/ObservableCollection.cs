using System.Collections.ObjectModel;

namespace ShortCuts_Manager.ExtensionMethods
{
    public static class ObservableCollection
    {
        public static void AddSorted<T, TKey>(this ObservableCollection<T> collection, T item, Func<T, TKey> keySelector)
        {
            int index = 0;
            while (index < collection.Count && Comparer<TKey>.Default.Compare(keySelector(collection[index]), keySelector(item)) < 0)
            {
                index++;
            }

            collection.Insert(index, item);
        }
    }
}
