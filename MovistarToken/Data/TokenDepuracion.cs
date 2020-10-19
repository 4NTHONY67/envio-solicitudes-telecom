﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovistarToken.Data
{
    public class TokenDepuracion
    {
        public int IdTokenDepuracion { get; set; }
        public string NombreContexto { get; set; }
        public string Estado { get; set; }
        public int PeriodoEjecucion { get; set; }
        public DateTime? FechaEjecucion { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}