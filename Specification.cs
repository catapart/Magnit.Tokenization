using System.Text.RegularExpressions;
using System.Collections;

namespace Magnit.Tokenization
{
    public sealed class Specification : IList<SpecificationItem>
    {
        private List<SpecificationItem> _specItems = new List<SpecificationItem>();
        public SpecificationItem this[int index] { get => _specItems[index]; set => _specItems[index] = value; }

        public int Count => _specItems.Count;

        public bool IsReadOnly { get; set; }

        public void Add(SpecificationItem item) => _specItems.Add(item);
        public void Add(Regex regex, string key) => _specItems.Add(new() { Regex = regex, Key = key });
        public void Add(Regex regex, string key, Func<string, Task<string>> resultCallback, string[]? captureGroups = null) => _specItems.Add(new() { Regex = regex, Key = key, ResultCallback = resultCallback, CaptureGroups = captureGroups });
        public void Add(Regex regex, string key, string[] captureGroups, Func<string, Task<string>>? resultCallback = null) => _specItems.Add(new() { Regex = regex, Key = key, CaptureGroups = captureGroups, ResultCallback = resultCallback });

        public void Clear() => _specItems.Clear();

        public bool Contains(SpecificationItem item) => _specItems.Contains(item);

        public void CopyTo(SpecificationItem[] array, int arrayIndex) => _specItems.CopyTo(array, arrayIndex);

        public IEnumerator<SpecificationItem> GetEnumerator() => _specItems.GetEnumerator();

        public int IndexOf(SpecificationItem item) => _specItems.IndexOf(item);

        public void Insert(int index, SpecificationItem item) => _specItems.Insert(index, item);

        public bool Remove(SpecificationItem item) => _specItems.Remove(item);

        public void RemoveAt(int index) => _specItems.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_specItems).GetEnumerator();
        }
    }
}