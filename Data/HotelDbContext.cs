using System;
using System.Collections.Generic;
using HotelGenericoApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelGenericoApi.Data;

public partial class HotelDbContext : DbContext
{
    public HotelDbContext()
    {
    }

    public HotelDbContext(DbContextOptions<HotelDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AfectacionIgv> AfectacionIgvs { get; set; }
    public virtual DbSet<CategoriaProducto> CategoriaProductos { get; set; }
    public virtual DbSet<CierreCajaEnvio> CierreCajaEnvios { get; set; }
    public virtual DbSet<Cliente> Clientes { get; set; }
    public virtual DbSet<Comprobante> Comprobantes { get; set; }
    public virtual DbSet<Configuracion> Configuracion { get; set; }
    public virtual DbSet<EstadoHabitacion> EstadosHabitacion { get; set; }
    public virtual DbSet<EstadoSunat> EstadosSunat { get; set; }
    public virtual DbSet<Estancium> Estancias { get; set; }
    public virtual DbSet<Habitacion> Habitaciones { get; set; }
    public virtual DbSet<HistorialEstadoHabitacion> HistorialesEstadoHabitacion { get; set; }
    public virtual DbSet<Huesped> Huespedes { get; set; }
    public virtual DbSet<ItemEstancium> ItemsEstancia { get; set; }
    public virtual DbSet<ItemVenta> ItemsVenta { get; set; }
    public virtual DbSet<MetodoPago> MetodosPago { get; set; }
    public virtual DbSet<Producto> Productos { get; set; }
    public virtual DbSet<Reserva> Reservas { get; set; }
    public virtual DbSet<RolUsuario> RolesUsuario { get; set; }
    public virtual DbSet<Tarifa> Tarifas { get; set; }
    public virtual DbSet<Temporada> Temporadas { get; set; }
    public virtual DbSet<TipoComprobante> TiposComprobante { get; set; }
    public virtual DbSet<TipoDocumento> TiposDocumento { get; set; }
    public virtual DbSet<TipoHabitacion> TiposHabitacion { get; set; }
    public virtual DbSet<TransicionEstado> TransicionesEstado { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<VCierreCajaDiario> VCierreCajaDiarios { get; set; }
    public virtual DbSet<VEstadoHabitacione> VEstadosHabitaciones { get; set; }
    public virtual DbSet<VOcupacionDiarium> VOcupacionDiaria { get; set; }
    public virtual DbSet<Venta> Ventas { get; set; }
    public virtual DbSet<LoginAttempt> LoginAttempts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseSqlServer("Name=DefaultConnection");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AfectacionIgv>(entity =>
        {
            entity.ToTable("afectacion_igv");
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(2).IsFixedLength();
            entity.Property(e => e.Descripcion).HasMaxLength(60);
        });

        modelBuilder.Entity<CategoriaProducto>(entity =>
        {
            entity.ToTable("categoria_producto");
            entity.HasKey(e => e.IdCategoria);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(100);
        });

        modelBuilder.Entity<CierreCajaEnvio>(entity =>
        {
            entity.ToTable("cierre_caja_envio");
            entity.HasKey(e => e.Fecha);
            entity.Property(e => e.IdEstadoSunat).HasDefaultValue(1);
            entity.Property(e => e.IntentosEnvio).HasDefaultValue(0);
            entity.HasOne(e => e.EstadoSunat).WithMany(es => es.CierreCajaEnvios).HasForeignKey(e => e.IdEstadoSunat);
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.ToTable("cliente");
            entity.HasKey(e => e.IdCliente);
            entity.HasIndex(e => new { e.TipoDocumento, e.Documento }).IsUnique();
            entity.Property(e => e.TipoDocumento).HasMaxLength(1).IsFixedLength();
            entity.Property(e => e.Documento).HasMaxLength(20);
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.Apellidos).HasMaxLength(100);
            entity.Property(e => e.Nacionalidad).HasMaxLength(50).HasDefaultValue("PERUANA");
            entity.Property(e => e.Telefono).HasMaxLength(15);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.HasOne(e => e.TipoDocumentoRel).WithMany(t => t.Clientes).HasForeignKey(e => e.TipoDocumento).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Comprobante>(entity =>
        {
            entity.ToTable("comprobante");
            entity.HasKey(e => e.IdComprobante);
            entity.HasIndex(e => new { e.Serie, e.Correlativo }).IsUnique();
            entity.Property(e => e.TipoComprobante).HasMaxLength(2).IsFixedLength();
            entity.Property(e => e.Serie).HasMaxLength(4);
            entity.Property(e => e.ClienteDocumentoTipo).HasMaxLength(1).IsFixedLength();
            entity.Property(e => e.ClienteDocumentoNum).HasMaxLength(20);
            entity.Property(e => e.ClienteNombre).HasMaxLength(200);
            entity.Property(e => e.MetodoPago).HasMaxLength(3).IsFixedLength();
            entity.Property(e => e.HashXml).HasMaxLength(64);
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(10,2)");
            entity.Property(e => e.IgvMonto).HasColumnType("decimal(10,2)");
            entity.Property(e => e.FechaEmision).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IdEstadoSunat).HasDefaultValue(1);
            entity.Property(e => e.IntentosEnvio).HasDefaultValue(0);
            entity.HasOne(e => e.TipoComprobanteRel).WithMany(t => t.Comprobantes).HasForeignKey(e => e.TipoComprobante).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.EstadoSunat).WithMany(es => es.Comprobantes).HasForeignKey(e => e.IdEstadoSunat).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.MetodoPagoRel).WithMany(m => m.Comprobantes).HasForeignKey(e => e.MetodoPago).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Configuracion>(entity =>
        {
            entity.ToTable("configuracion");
            entity.HasKey(e => e.IdConfiguracion);
            entity.Property(e => e.IdConfiguracion).HasDefaultValue(1);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Direccion).HasMaxLength(200);
            entity.Property(e => e.Ruc).HasMaxLength(11);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.TasaIgvHotel).HasColumnType("decimal(5,2)").HasDefaultValue(18.00m);
            entity.Property(e => e.TasaIgvProductos).HasColumnType("decimal(5,2)").HasDefaultValue(18.00m);
        });

        modelBuilder.Entity<EstadoHabitacion>(entity =>
        {
            entity.ToTable("estado_habitacion");
            entity.HasKey(e => e.IdEstado);
            entity.Property(e => e.Nombre).HasMaxLength(30);
            entity.Property(e => e.Descripcion).HasMaxLength(100);
            entity.Property(e => e.ColorUi).HasMaxLength(20);
            entity.HasMany(e => e.Habitaciones).WithOne(h => h.Estado).HasForeignKey(e => e.IdEstado).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasMany(e => e.TransicionesComoEstadoActual).WithOne(t => t.EstadoActual).HasForeignKey(e => e.IdEstadoActual).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasMany(e => e.TransicionesComoEstadoSiguiente).WithOne(t => t.EstadoSiguiente).HasForeignKey(e => e.IdEstadoSiguiente).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EstadoSunat>(entity =>
        {
            entity.ToTable("estado_sunat");
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Descripcion).HasMaxLength(60);
            entity.Property(e => e.DescripcionLarga).HasMaxLength(200);
        });

        modelBuilder.Entity<Estancium>(entity =>
        {
            entity.ToTable("estancia");
            entity.HasKey(e => e.IdEstancia);
            entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("Activa");
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.HasOne(e => e.ClienteTitular).WithMany(c => c.Estancias).HasForeignKey(e => e.IdClienteTitular).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.Habitacion).WithMany(h => h.Estancias).HasForeignKey(e => e.IdHabitacion).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.Reserva).WithMany(r => r.Estancias).HasForeignKey(e => e.IdReserva);
            entity.HasMany(e => e.Huespedes).WithOne(h => h.Estancia).HasForeignKey(e => e.IdEstancia).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasMany(e => e.ItemsEstancia).WithOne(i => i.Estancia).HasForeignKey(e => e.IdEstancia).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Habitacion>(entity =>
        {
            entity.ToTable("habitacion");
            entity.HasKey(e => e.IdHabitacion);
            entity.HasIndex(e => e.NumeroHabitacion).IsUnique();
            entity.Property(e => e.NumeroHabitacion).HasMaxLength(10);
            entity.Property(e => e.Piso).HasDefaultValue(1);
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.PrecioNoche).HasColumnType("decimal(10,2)").HasDefaultValue(50.00m);
            entity.Property(e => e.IdEstado).HasDefaultValue(1);
            entity.Property(e => e.FechaUltimoCambio).HasDefaultValueSql("(getdate())");
            entity.HasOne(e => e.Estado).WithMany(est => est.Habitaciones).HasForeignKey(e => e.IdEstado).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.TipoHabitacion).WithMany(th => th.Habitaciones).HasForeignKey(e => e.IdTipo).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.UsuarioCambioRel).WithMany(u => u.HabitacionesModificadas).HasForeignKey(e => e.UsuarioCambio);
            entity.HasMany(e => e.HistorialesEstadoHabitacion).WithOne(h => h.Habitacion).HasForeignKey(e => e.IdHabitacion).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasMany(e => e.Reservas).WithOne(r => r.Habitacion).HasForeignKey(e => e.IdHabitacion).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<HistorialEstadoHabitacion>(entity =>
        {
            entity.ToTable("historial_estado_habitacion");
            entity.HasKey(e => e.IdHistorial);
            entity.HasIndex(e => new { e.IdHabitacion, e.FechaCambio });
            entity.Property(e => e.Observacion).HasMaxLength(200);
            entity.Property(e => e.FechaCambio).HasDefaultValueSql("(getdate())");
            entity.HasOne(e => e.Habitacion).WithMany(h => h.HistorialesEstadoHabitacion).HasForeignKey(e => e.IdHabitacion).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.Usuario).WithMany(u => u.HistorialesEstadoHabitacion).HasForeignKey(e => e.IdUsuario);
        });

        modelBuilder.Entity<Huesped>(entity =>
        {
            entity.ToTable("huesped");
            entity.HasKey(e => e.IdHuesped);
            entity.Property(e => e.EsTitular).HasDefaultValue(false);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.HasOne(e => e.Cliente).WithMany(c => c.Huespedes).HasForeignKey(e => e.IdCliente).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.Estancia).WithMany(e => e.Huespedes).HasForeignKey(e => e.IdEstancia).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ItemEstancium>(entity =>
        {
            entity.ToTable("item_estancia");
            entity.HasKey(e => e.IdItem);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(21,2)").HasComputedColumnSql("([cantidad]*[precio_unitario])", true);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.HasOne(e => e.Estancia).WithMany(est => est.ItemsEstancia).HasForeignKey(e => e.IdEstancia).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.Producto).WithMany(p => p.ItemsEstancia).HasForeignKey(e => e.IdProducto).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ItemVenta>(entity =>
        {
            entity.ToTable("item_venta");
            entity.HasKey(e => e.IdItem);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Subtotal).HasColumnType("decimal(21,2)").HasComputedColumnSql("([cantidad]*[precio_unitario])", true);
            entity.HasOne(e => e.Producto).WithMany(p => p.ItemsVenta).HasForeignKey(e => e.IdProducto).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.Venta).WithMany(v => v.ItemsVenta).HasForeignKey(e => e.IdVenta).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MetodoPago>(entity =>
        {
            entity.ToTable("metodo_pago");
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(3).IsFixedLength();
            entity.Property(e => e.Descripcion).HasMaxLength(60);
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.ToTable("producto");
            entity.HasKey(e => e.IdProducto);
            entity.HasIndex(e => e.CodigoSunat);
            entity.Property(e => e.CodigoSunat).HasMaxLength(20);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(10,2)");
            entity.Property(e => e.IdAfectacionIgv).HasMaxLength(2).IsFixedLength().HasDefaultValue("10");
            entity.Property(e => e.UnidadMedida).HasMaxLength(3).HasDefaultValue("NIU");
            entity.Property(e => e.Stock).HasDefaultValue(0);
            entity.Property(e => e.StockMinimo).HasDefaultValue(5);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.HasOne(e => e.AfectacionIgv).WithMany(a => a.Productos).HasForeignKey(e => e.IdAfectacionIgv).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.Categoria).WithMany(c => c.Productos).HasForeignKey(e => e.IdCategoria);
            entity.HasMany(e => e.ItemsEstancia).WithOne(i => i.Producto).HasForeignKey(e => e.IdProducto);
            entity.HasMany(e => e.ItemsVenta).WithOne(i => i.Producto).HasForeignKey(e => e.IdProducto);
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.ToTable("reserva");
            entity.HasKey(e => e.IdReserva);
            entity.Property(e => e.Estado).HasMaxLength(20).HasDefaultValue("Pendiente");
            entity.Property(e => e.MontoTotal).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Observaciones).HasMaxLength(300);
            entity.Property(e => e.FechaRegistro).HasDefaultValueSql("(getdate())");
            entity.HasOne(e => e.Cliente).WithMany(c => c.Reservas).HasForeignKey(e => e.IdCliente).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.Habitacion).WithMany(h => h.Reservas).HasForeignKey(e => e.IdHabitacion).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.Usuario).WithMany(u => u.Reservas).HasForeignKey(e => e.IdUsuario).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasMany(e => e.Estancias).WithOne(e => e.Reserva).HasForeignKey(e => e.IdReserva);
        });

        modelBuilder.Entity<RolUsuario>(entity =>
        {
            entity.ToTable("rol_usuario");
            entity.HasKey(e => e.IdRol);
            entity.Property(e => e.Nombre).HasMaxLength(30);
            entity.HasMany(e => e.Usuarios).WithOne(u => u.Rol).HasForeignKey(e => e.IdRol).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Tarifa>(entity =>
        {
            entity.ToTable("tarifa");
            entity.HasKey(e => e.IdTarifa);
            entity.Property(e => e.Precio).HasColumnType("decimal(10,2)");
            entity.HasOne(e => e.TipoHabitacion).WithMany(th => th.Tarifas).HasForeignKey(e => e.IdTipoHabitacion).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.Temporada).WithMany(t => t.Tarifas).HasForeignKey(e => e.IdTemporada);
        });

        modelBuilder.Entity<Temporada>(entity =>
        {
            entity.ToTable("temporada");
            entity.HasKey(e => e.IdTemporada);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Multiplicador).HasColumnType("decimal(3,2)").HasDefaultValue(1.00m);
            entity.HasMany(e => e.Tarifas).WithOne(t => t.Temporada).HasForeignKey(e => e.IdTemporada);
        });

        modelBuilder.Entity<TipoComprobante>(entity =>
        {
            entity.ToTable("tipo_comprobante");
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(2).IsFixedLength();
            entity.Property(e => e.Descripcion).HasMaxLength(60);
        });

        modelBuilder.Entity<TipoDocumento>(entity =>
        {
            entity.ToTable("tipo_documento");
            entity.HasKey(e => e.Codigo);
            entity.Property(e => e.Codigo).HasMaxLength(1).IsFixedLength();
            entity.Property(e => e.Descripcion).HasMaxLength(60);
            entity.HasMany(e => e.Clientes).WithOne(c => c.TipoDocumentoRel).HasForeignKey(e => e.TipoDocumento);
        });

        modelBuilder.Entity<TipoHabitacion>(entity =>
        {
            entity.ToTable("tipo_habitacion");
            entity.HasKey(e => e.IdTipo);
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Capacidad).HasDefaultValue(2);
            entity.Property(e => e.PrecioBase).HasColumnType("decimal(10,2)").HasDefaultValue(50.00m);
            entity.HasMany(e => e.Habitaciones).WithOne(h => h.TipoHabitacion).HasForeignKey(e => e.IdTipo).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasMany(e => e.Tarifas).WithOne(t => t.TipoHabitacion).HasForeignKey(e => e.IdTipoHabitacion);
        });

        modelBuilder.Entity<TransicionEstado>(entity =>
        {
            entity.ToTable("transicion_estado");
            entity.HasKey(e => e.IdTransicion);
            entity.HasIndex(e => new { e.IdEstadoActual, e.IdEstadoSiguiente }).IsUnique();
            entity.HasOne(e => e.EstadoActual).WithMany(est => est.TransicionesComoEstadoActual).HasForeignKey(e => e.IdEstadoActual).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.EstadoSiguiente).WithMany(est => est.TransicionesComoEstadoSiguiente).HasForeignKey(e => e.IdEstadoSiguiente).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("usuario");
            entity.HasKey(e => e.IdUsuario);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).HasMaxLength(50);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.FechaCreacion).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.EstaActivo).HasDefaultValue(true);
            entity.HasOne(e => e.Rol).WithMany(r => r.Usuarios).HasForeignKey(e => e.IdRol).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasMany(e => e.Reservas).WithOne(r => r.Usuario).HasForeignKey(e => e.IdUsuario);
            entity.HasMany(e => e.Ventas).WithOne(v => v.Usuario).HasForeignKey(e => e.IdUsuario).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<VCierreCajaDiario>(entity =>
        {
            entity.HasNoKey().ToView("v_cierre_caja_diario");
            entity.Property(e => e.Concepto).HasMaxLength(9);
        });

        modelBuilder.Entity<VEstadoHabitacione>(entity =>
        {
            entity.HasNoKey().ToView("v_estado_habitaciones");
            entity.Property(e => e.NumeroHabitacion).HasMaxLength(10);
            entity.Property(e => e.Estado).HasMaxLength(30);
            entity.Property(e => e.TipoHabitacion).HasMaxLength(50);
            entity.Property(e => e.PrecioNoche).HasColumnType("decimal(10,2)");
        });

        modelBuilder.Entity<VOcupacionDiarium>(entity =>
        {
            entity.HasNoKey().ToView("v_ocupacion_diaria");
            entity.Property(e => e.PorcentajeOcupacion).HasColumnType("decimal(5,2)");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.ToTable("venta");
            entity.HasKey(e => e.IdVenta);
            entity.Property(e => e.Total).HasColumnType("decimal(10,2)");
            entity.Property(e => e.MetodoPago).HasMaxLength(3).IsFixedLength();
            entity.Property(e => e.FechaVenta).HasDefaultValueSql("(getdate())");
            entity.HasOne(e => e.Cliente).WithMany(c => c.Ventas).HasForeignKey(e => e.IdCliente);
            entity.HasOne(e => e.Usuario).WithMany(u => u.Ventas).HasForeignKey(e => e.IdUsuario).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(e => e.MetodoPagoRel).WithMany(m => m.Ventas).HasForeignKey(e => e.MetodoPago).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasMany(e => e.ItemsVenta).WithOne(i => i.Venta).HasForeignKey(e => e.IdVenta);
        });

        modelBuilder.Entity<LoginAttempt>(entity =>
        {
            entity.ToTable("login_attempt");
            entity.HasKey(e => e.IdLoginAttempt);
            entity.Property(e => e.IpAddress).HasMaxLength(50);
            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            entity.Property(e => e.AttemptedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Succeeded).HasDefaultValue(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}