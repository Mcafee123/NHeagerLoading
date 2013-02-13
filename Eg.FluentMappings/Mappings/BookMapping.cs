using FluentNHibernate.Mapping;

namespace Eg.FluentMappings.Mappings
{
    public class BookMapping: SubclassMap<Book>
    {
        public BookMapping()
        {
            Map(p => p.Author);
            Map(p => p.ISBN);
        }
    }
}
