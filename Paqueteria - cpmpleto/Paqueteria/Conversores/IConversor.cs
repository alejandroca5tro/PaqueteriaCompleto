using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paqueteria.Conversores
{
    public interface IConversor<T, TDTO>
    {
        TDTO Entity2DTO(T entity);
        T DTO2Entity(TDTO dto);
    }
}
