using System;
using System.Collections.Generic;

namespace LogisControlAPI.Models
{
    /// <summary>
    /// Representa uma entidade de assistência externa que pode estar associada a máquinas ou registos de manutenção.
    /// </summary>
    public partial class AssistenciaExterna
    {
        /// <summary>
        /// Identificador único da assistência externa.
        /// </summary>
        public int AssistenteId { get; set; }

        /// <summary>
        /// Nome da empresa ou pessoa responsável pela assistência externa.
        /// </summary>
        public string Nome { get; set; } = null!;

        /// <summary>
        /// Número de Identificação Fiscal (NIF) da assistência externa.
        /// </summary>
        public int Nif { get; set; }

        /// <summary>
        /// Morada (endereço) da assistência externa.
        /// </summary>
        public string Morada { get; set; } = null!;

        /// <summary>
        /// Número de telefone de contacto da assistência externa.
        /// </summary>
        public int Telefone { get; set; }

        /// <summary>
        /// Lista de máquinas associadas a esta assistência externa.
        /// </summary>
        public virtual ICollection<Maquina> Maquinas { get; set; } = new List<Maquina>();

        /// <summary>
        /// Lista de registos de manutenção realizados por esta assistência externa.
        /// </summary>
        public virtual ICollection<RegistoManutencao> RegistoManutencaos { get; set; } = new List<RegistoManutencao>();
    }
}