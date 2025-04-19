using System;
using System.Collections.Generic;
using LogisControlAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LogisControlAPI.Data;

public partial class LogisControlContext : DbContext
{
    public LogisControlContext()
    {
    }

    public LogisControlContext(DbContextOptions<LogisControlContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AssistenciaExterna> AssistenciasExternas { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<EncomendaCliente> EncomendasCliente { get; set; }

    public virtual DbSet<EncomendaItens> EncomendasItem { get; set; }

    public virtual DbSet<Fornecedor> Fornecedores { get; set; }

    public virtual DbSet<Maquina> Maquinas { get; set; }

    public virtual DbSet<MateriaPrima> MateriasPrimas { get; set; }

    public virtual DbSet<MateriaPrimaProduto> MateriaPrimaProdutos { get; set; }

    public virtual DbSet<NotaEncomendaItens> NotasEncomendaItem { get; set; }

    public virtual DbSet<NotaEncomenda> NotasEncomenda { get; set; }

    public virtual DbSet<Orcamento> Orcamentos { get; set; }

    public virtual DbSet<OrcamentoItem> OrcamentosItem { get; set; }

    public virtual DbSet<OrdemProducao> OrdensProducao { get; set; }

    public virtual DbSet<PedidoCompra> PedidosCompra { get; set; }

    public virtual DbSet<PedidoCotacao> PedidosCotacao { get; set; }

    public virtual DbSet<PedidoManutencao> PedidosManutencao { get; set; }

    public virtual DbSet<ProdMateriais> ProdMateriais { get; set; }

    public virtual DbSet<Produto> Produtos { get; set; }

    public virtual DbSet<RegistoManutencao> RegistosManutencao { get; set; }

    public virtual DbSet<RegistoProducao> RegistosProducao { get; set; }

    public virtual DbSet<Utilizador> Utilizadores { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssistenciaExterna>(entity =>
        {
            entity.HasKey(e => e.AssistenteId).HasName("PK__Assisten__B59C5619B698B885");

            entity.ToTable("AssistenciaExterna");

            entity.Property(e => e.AssistenteId).HasColumnName("AssistenteID");
            entity.Property(e => e.Morada)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nif).HasColumnName("NIF");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.ClienteId).HasName("PK__Cliente__71ABD0A7BE7C225E");

            entity.ToTable("Cliente");

            entity.Property(e => e.ClienteId).HasColumnName("ClienteID");
            entity.Property(e => e.Morada)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nif).HasColumnName("NIF");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EncomendaCliente>(entity =>
        {
            entity.HasKey(e => e.EncomendaClienteId).HasName("PK__Encomend__A47F61650A4349A5");

            entity.ToTable("EncomendaCliente");

            entity.Property(e => e.EncomendaClienteId).HasColumnName("EncomendaClienteID");
            entity.Property(e => e.ClienteClienteId).HasColumnName("ClienteClienteID");
            entity.Property(e => e.DataEncomenda).HasColumnType("datetime");
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.ClienteCliente).WithMany(p => p.EncomendasCliente)
                .HasForeignKey(d => d.ClienteClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKEncomendaC556375");
        });

        modelBuilder.Entity<EncomendaItens>(entity =>
        {
            entity.HasKey(e => e.EncomendaItensId).HasName("PK__Encomend__A1D00A02E7FB7FED");

            entity.ToTable("EncomendaItens");

            entity.Property(e => e.EncomendaClienteEncomendaClienteId).HasColumnName("EncomendaClienteEncomendaClienteID");

            entity.HasOne(d => d.EncomendaClienteEncomendaCliente).WithMany(p => p.EncomendasItem)
                .HasForeignKey(d => d.EncomendaClienteEncomendaClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKEncomendaI428182");
        });

        modelBuilder.Entity<Fornecedor>(entity =>
        {
            entity.HasKey(e => e.FornecedorId).HasName("PK__Forneced__494B8C3028ECCE34");

            entity.ToTable("Fornecedor");

            entity.Property(e => e.FornecedorId).HasColumnName("FornecedorID");
            entity.Property(e => e.Email)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Maquina>(entity =>
        {
            entity.HasKey(e => e.MaquinaId).HasName("PK__Maquina__5D47B8B5988D66C8");

            entity.ToTable("Maquina");

            entity.Property(e => e.MaquinaId).HasColumnName("MaquinaID");
            entity.Property(e => e.AssistenciaExternaAssistenteId).HasColumnName("AssistenciaExternaAssistenteID");
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.HasOne(d => d.AssistenciaExternaAssistente).WithMany(p => p.Maquinas)
                .HasForeignKey(d => d.AssistenciaExternaAssistenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKMaquina431467");
        });

        modelBuilder.Entity<MateriaPrima>(entity =>
        {
            entity.HasKey(e => e.MateriaPrimaId).HasName("PK__MateriaP__5C2641D25D8355F5");

            entity.ToTable("MateriaPrima");

            entity.Property(e => e.MateriaPrimaId).HasColumnName("MateriaPrimaID");
            entity.HasIndex(e => e.CodInterno, "UQ__MateriaP__39F955FCFF814842").IsUnique();

            entity.Property(e => e.Categoria)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.CodInterno)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Descricao)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        modelBuilder.Entity<MateriaPrimaProduto>(entity =>
        {
            entity.HasKey(e => e.MateriaPrimaProdutoId).HasName("PK__MateriaP__423F3868D5F56872");

            entity.ToTable("MateriaPrimaProduto");

            entity.Property(e => e.MateriaPrimaProdutoId).HasColumnName("MateriaPrimaProdutoID");
            entity.Property(e => e.ProdutoProdutoId).HasColumnName("ProdutoProdutoID");
            entity.Property(e => e.MateriaPrimaMateriaPrimaId).HasColumnName("MateriaPrimaMateriaPrimaID");

            entity.HasOne(d => d.MateriaPrimaMateriaPrimaIDNavigation).WithMany(p => p.MateriaPrimaProdutos)
                .HasForeignKey(d => d.MateriaPrimaMateriaPrimaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKMateriaPri691498");

            entity.HasOne(d => d.ProdutoProduto).WithMany(p => p.MateriaPrimaProdutos)
                .HasForeignKey(d => d.ProdutoProdutoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKMateriaPri932813");
        });

        modelBuilder.Entity<NotaEncomendaItens>(entity =>
        {
            entity.HasKey(e => e.NotaEncomendaItensId).HasName("PK__NotaEnco__988B175BEF17ADA7");

            entity.Property(e => e.NotaEncomendaItensId).HasColumnName("NotaEncomendaItensID");
            entity.Property(e => e.NotaEncomendaNotaEncomendaId).HasColumnName("NotaEncomendaNotaEncomendaID");

            entity.HasOne(d => d.MateriaPrimaMateriaPrimaIDNavigation).WithMany(p => p.NotasEncomendaItem)
                .HasForeignKey(d => d.MateriaPrimaMateriaPrimaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKNotaEncome850921");

            entity.HasOne(d => d.NotaEncomendaNotaEncomenda).WithMany(p => p.NotasEncomendaItem)
                .HasForeignKey(d => d.NotaEncomendaNotaEncomendaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKNotaEncome419194");
        });

        modelBuilder.Entity<NotaEncomenda>(entity =>
        {
            entity.HasKey(e => e.NotaEncomendaId).HasName("PK__NotaEnco__9B94B6476CB9B6CB");

            entity.Property(e => e.NotaEncomendaId).HasColumnName("NotaEncomendaID");
            entity.Property(e => e.DataEmissao).HasColumnType("datetime");
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.OrcamentoOrcamentoId).HasColumnName("OrcamentoOrcamentoID");

            entity.HasOne(d => d.OrcamentoOrcamento).WithMany(p => p.NotaEncomenda)
                .HasForeignKey(d => d.OrcamentoOrcamentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKNotaEncome702864");
        });

        modelBuilder.Entity<Orcamento>(entity =>
        {
            entity.HasKey(e => e.OrcamentoId).HasName("PK__Orcament__4E96F759D136FF91");

            entity.ToTable("Orcamento");

            entity.Property(e => e.OrcamentoId).HasColumnName("OrcamentoID");
            entity.Property(e => e.Data).HasColumnType("datetime");
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PedidoCotacaoPedidoCotacaoId).HasColumnName("PedidoCotacaoPedidoCotacaoID");

            entity.HasOne(d => d.PedidoCotacaoPedidoCotacao).WithMany(p => p.Orcamentos)
                .HasForeignKey(d => d.PedidoCotacaoPedidoCotacaoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKOrcamento236624");
        });

        modelBuilder.Entity<OrcamentoItem>(entity =>
        {
            entity.HasKey(e => e.OrcamentoItemId).HasName("PK__Orcament__8AEC7376D11B449F");

            entity.ToTable("OrcamentoItem");

            entity.Property(e => e.OrcamentoItemId).HasColumnName("OrcamentoItemID");
            entity.Property(e => e.OrcamentoOrcamentoId).HasColumnName("OrcamentoOrcamentoID");

            entity.HasOne(d => d.MateriaPrimaMateriaPrimaIDNavigation).WithMany(p => p.OrcamentosItem)
                .HasForeignKey(d => d.MateriaPrimaMateriaPrimaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKOrcamentoI247272");

            entity.HasOne(d => d.OrcamentoOrcamento).WithMany(p => p.OrcamentosItem)
                .HasForeignKey(d => d.OrcamentoOrcamentoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKOrcamentoI811131");
        });

        modelBuilder.Entity<OrdemProducao>(entity =>
        {
            entity.HasKey(e => e.OrdemProdId).HasName("PK__OrdemPro__8DBE0AA93037B167");

            entity.ToTable("OrdemProducao");

            entity.Property(e => e.OrdemProdId).HasColumnName("OrdemProdID");
            entity.Property(e => e.DataAbertura).HasColumnType("datetime");
            entity.Property(e => e.DataConclusao).HasColumnType("datetime");
            entity.Property(e => e.EncomendaClienteEncomendaClienteId).HasColumnName("EncomendaClienteEncomendaClienteID");
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaquinaMaquinaId).HasColumnName("MaquinaMaquinaID");

            entity.HasOne(d => d.EncomendaClienteEncomendaCliente).WithMany(p => p.OrdensProducao)
                .HasForeignKey(d => d.EncomendaClienteEncomendaClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKOrdemProdu255309");

            entity.HasOne(d => d.MaquinaMaquina).WithMany(p => p.OrdensProducao)
                .HasForeignKey(d => d.MaquinaMaquinaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKOrdemProdu935284");
        });

        modelBuilder.Entity<PedidoCompra>(entity =>
        {
            entity.HasKey(e => e.PedidoCompraId).HasName("PK__PedidoCo__C4447AAC893F9D8B");

            entity.ToTable("PedidoCompra");

            entity.Property(e => e.PedidoCompraId).HasColumnName("PedidoCompraID");
            entity.Property(e => e.DataAbertura).HasColumnType("datetime");
            entity.Property(e => e.DataConclusao).HasColumnType("datetime");
            entity.Property(e => e.Descricao)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.UtilizadorUtilizadorId).HasColumnName("UtilizadorUtilizadorID");

            entity.HasOne(d => d.UtilizadorUtilizador).WithMany(p => p.PedidosCompra)
                .HasForeignKey(d => d.UtilizadorUtilizadorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPedidoComp502741");
        });

        modelBuilder.Entity<PedidoCotacao>(entity =>
        {
            entity.HasKey(e => e.PedidoCotacaoId).HasName("PK__PedidoCo__C1AE3DCE1957AE9C");

            entity.ToTable("PedidoCotacao");

            entity.Property(e => e.PedidoCotacaoId).HasColumnName("PedidoCotacaoID");
            entity.Property(e => e.Data).HasColumnType("datetime");
            entity.Property(e => e.Descricao)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.FornecedorFornecedorId).HasColumnName("FornecedorFornecedorID");

            entity.HasOne(d => d.FornecedorFornecedor).WithMany(p => p.PedidosCotacao)
                .HasForeignKey(d => d.FornecedorFornecedorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPedidoCota347557");
        });

        modelBuilder.Entity<PedidoManutencao>(entity =>
        {
            entity.HasKey(e => e.PedidoManutId).HasName("PK__PedidoMa__CEC3FA7F29D627E6");

            entity.ToTable("PedidoManutencao");

            entity.Property(e => e.PedidoManutId).HasColumnName("PedidoManutID");
            entity.Property(e => e.DataAbertura).HasColumnType("datetime");
            entity.Property(e => e.DataConclusao).HasColumnType("datetime");
            entity.Property(e => e.Descricao)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaquinaMaquinaId).HasColumnName("MaquinaMaquinaID");
            entity.Property(e => e.UtilizadorUtilizadorId).HasColumnName("UtilizadorUtilizadorID");

            entity.HasOne(d => d.MaquinaMaquina).WithMany(p => p.PedidosManutencao)
                .HasForeignKey(d => d.MaquinaMaquinaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPedidoManu589131");

            entity.HasOne(d => d.UtilizadorUtilizador).WithMany(p => p.PedidosManutencao)
                .HasForeignKey(d => d.UtilizadorUtilizadorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKPedidoManu67204");
        });

        modelBuilder.Entity<ProdMateriais>(entity =>
        {
            entity.HasKey(e => e.ProdMateriaisId).HasName("PK__ProdMate__812930CE2175FA55");

            entity.Property(e => e.ProdMateriaisId).HasColumnName("ProdMateriaisID");
            entity.Property(e => e.OrdemProducaoOrdemProdId).HasColumnName("OrdemProducaoOrdemProdID");

            entity.HasOne(d => d.MateriaPrimaMateriaPrimaIDNavigation).WithMany(p => p.ProdMateriais)
                .HasForeignKey(d => d.MateriaPrimaMateriaPrimaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProdMateri754466");

            entity.HasOne(d => d.OrdemProducaoOrdemProd).WithMany(p => p.ProdMateriais)
                .HasForeignKey(d => d.OrdemProducaoOrdemProdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProdMateri73484");
        });

        modelBuilder.Entity<Produto>(entity =>
        {
            entity.HasKey(e => e.ProdutoId).HasName("PK__Produto__9C8800C3FF800782");

            entity.ToTable("Produto");

            entity.Property(e => e.ProdutoId).HasColumnName("ProdutoID");
            entity.Property(e => e.CodInterno)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Descricao)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OrdemProducaoOrdemProdId).HasColumnName("OrdemProducaoOrdemProdID");
            entity.Property(e => e.Quantidade)
                .HasMaxLength(1000)
                .IsUnicode(false);

            entity.HasOne(d => d.EncomendaItensEncomendaItensNavigation).WithMany(p => p.Produtos)
                .HasForeignKey(d => d.EncomendaItensEncomendaItensId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProduto171408");

            entity.HasOne(d => d.OrdemProducaoOrdemProd).WithMany(p => p.Produtos)
                .HasForeignKey(d => d.OrdemProducaoOrdemProdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKProduto647431");
        });

        modelBuilder.Entity<RegistoManutencao>(entity =>
        {
            entity.HasKey(e => e.RegistoManutencaoId).HasName("PK__RegistoM__F5200D304E7EC19E");

            entity.ToTable("RegistoManutencao");

            entity.Property(e => e.RegistoManutencaoId).HasColumnName("RegistoManutencaoID");
            entity.Property(e => e.AssistenciaExternaAssistenteId).HasColumnName("AssistenciaExternaAssistenteID");
            entity.Property(e => e.Descricao)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.PedidoManutencaoPedidoManutId).HasColumnName("PedidoManutencaoPedidoManutID");
            entity.Property(e => e.UtilizadorUtilizadorId).HasColumnName("UtilizadorUtilizadorID");

            entity.HasOne(d => d.AssistenciaExternaAssistente).WithMany(p => p.RegistosManutencao)
                .HasForeignKey(d => d.AssistenciaExternaAssistenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKRegistoMan998094");

            entity.HasOne(d => d.PedidoManutencaoPedidoManut).WithMany(p => p.RegistosManutencao)
                .HasForeignKey(d => d.PedidoManutencaoPedidoManutId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKRegistoMan791617");

            entity.HasOne(d => d.UtilizadorUtilizador).WithMany(p => p.RegistosManutencao)
                .HasForeignKey(d => d.UtilizadorUtilizadorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKRegistoMan601844");
        });

        modelBuilder.Entity<RegistoProducao>(entity =>
        {
            entity.HasKey(e => e.RegistoProducaoId).HasName("PK__RegistoP__9350BF54B7C3CB14");

            entity.ToTable("RegistoProducao");

            entity.Property(e => e.RegistoProducaoId).HasColumnName("RegistoProducaoID");
            entity.Property(e => e.DataProducao).HasColumnType("datetime");
            entity.Property(e => e.Estado)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.Observacoes)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.OrdemProducaoOrdemProdId).HasColumnName("OrdemProducaoOrdemProdID");
            entity.Property(e => e.ProdutoProdutoId).HasColumnName("ProdutoProdutoID");
            entity.Property(e => e.UtilizadorUtilizadorId).HasColumnName("UtilizadorUtilizadorID");

            entity.HasOne(d => d.OrdemProducaoOrdemProd).WithMany(p => p.RegistosProducao)
                .HasForeignKey(d => d.OrdemProducaoOrdemProdId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKRegistoPro913410");

            entity.HasOne(d => d.ProdutoProduto).WithMany(p => p.RegistosProducao)
                .HasForeignKey(d => d.ProdutoProdutoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKRegistoPro11086");

            entity.HasOne(d => d.UtilizadorUtilizador).WithMany(p => p.RegistosProducao)
                .HasForeignKey(d => d.UtilizadorUtilizadorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKRegistoPro479439");
        });

        modelBuilder.Entity<Utilizador>(entity =>
        {
            entity.HasKey(e => e.UtilizadorId).HasName("PK__Utilizad__90F8E1C87750948F");

            entity.ToTable("Utilizador");

            entity.HasIndex(e => e.NumFuncionario, "UQ__Utilizad__A24D69839DDECD85").IsUnique();

            entity.Property(e => e.UtilizadorId).HasColumnName("UtilizadorID");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.PrimeiroNome)
                .HasMaxLength(25)
                .IsUnicode(false);
            entity.Property(e => e.Role)
                .HasMaxLength(15)
                .IsUnicode(false);
            entity.Property(e => e.Sobrenome)
                .HasMaxLength(25)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
