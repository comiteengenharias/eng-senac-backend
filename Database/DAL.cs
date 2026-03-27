using EngenhariasSenac.Database;

namespace EngenhariasSenac.Banco;
internal class DAL<T> where T : class
{
    protected readonly EngenhariasSenacContext context;

    public DAL(EngenhariasSenacContext context)
    {
        this.context = context;
    }

    public IEnumerable<T> Select(Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        IQueryable<T> query = context.Set<T>();
        return orderBy != null ? orderBy(query).ToList() : query.ToList();
    }

    public void Insert(T objeto)
    {
        context.Set<T>().Add(objeto);
        context.SaveChanges();
    }

    public void Update(T objeto)
    {
        context.Set<T>().Update(objeto);
        context.SaveChanges();
    }

    public void Delete(T objeto)
    {
        context.Set<T>().Remove(objeto);
        context.SaveChanges();
    }

    public T? SelectWhere(Func<T, bool> condicao)
    {
        return context.Set<T>().FirstOrDefault(condicao);
    }

    public List<T> SelectWhereList(Func<T, bool> condicao, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null)
    {
        IQueryable<T> query = context.Set<T>().Where(condicao).AsQueryable();
        return orderBy != null ? orderBy(query).ToList() : query.ToList();
    }
}