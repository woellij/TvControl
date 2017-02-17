using AutoMapper;

namespace TvControl.Player.App.ViewModels
{
    internal class IndexedItem<T>
    {

        private IndexedItem(T @object, int position)
        {
            this.Object = @object;
            this.Position = position;
        }

        public T Object { get; }
        public int Position { get; }

        public static IndexedItem<T> Create(T obj, int index)
        {
            return new IndexedItem<T>(obj, index);
        }

        public TTo To<TTo>()
        {
            TTo to = Mapper.Map<TTo>(this.Object);
            Mapper.Map(this, to);
            return to;
        }

    }
}