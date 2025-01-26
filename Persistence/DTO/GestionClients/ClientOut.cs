using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.DTO.GestionClients
{
    public  class ClientOut
    {
        public int Id { get; set; }
        public string nom { get; set; }
        public string address { get; set; }
        public int telephone { get; set; }
        public float note { get; set; }
        public bool estRestreint { get; set; } = false;

    }
}
