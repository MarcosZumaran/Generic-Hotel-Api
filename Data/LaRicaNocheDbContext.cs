using System;
using System.Collections.Generic;
using LaRicaNoche.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace LaRicaNoche.Api.Data;

public partial class LaRicaNocheDbContext : DbContext
{
    public LaRicaNocheDbContext()
    {
    }

    public LaRicaNocheDbContext(DbContextOptions<LaRicaNocheDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CatAfectacionIgv> CatAfectacionIgvs { get; set; }

    public virtual DbSet<CatEstadoHabitacion> CatEstadoHabitacions { get; set; }

    public virtual DbSet<CatEstadoSunat> CatEstadoSunats { get; set; }

    public virtual DbSet<CatMetodoPago> CatMetodoPagos { get; set; }

    public virtual DbSet<CatRolUsuario> CatRolUsuarios { get; set; }

    public virtual DbSet<CatTipoComprobante> CatTipoComprobantes { get; set; }

    public virtual DbSet<CatTipoDocumento> CatTipoDocumentos { get; set; }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Comprobante> Comprobantes { get; set; }

    public virtual DbSet<Estancia> Estancias { get; set; }

    public virtual DbSet<Habitacione> Habitaciones { get; set; }

    public virtual DbSet<HistorialEstadoHabitacion> HistorialEstadoHabitacions { get; set; }

    public virtual DbSet<Huespede> Huespedes { get; set; }

    public virtual DbSet<ItemsVentum> ItemsVenta { get; set; }

    public virtual DbSet<Producto> Productos { get; set; }

    public virtual DbSet<Reserva> Reservas { get; set; }

    public virtual DbSet<TiposHabitacion> TiposHabitacions { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<VCierreCajaDiario> VCierreCajaDiarios { get; set; }

    public virtual DbSet<VEstadoHabitacione> VEstadoHabitaciones { get; set; }

    public virtual DbSet<Venta> Ventas { get; set; }

    public virtual DbSet<CierreCajaEnvio> CierreCajaEnvios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatAfectacionIgv>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__cat_afec__40F9A2076CE4659E");

            entity.ToTable("cat_afectacion_igv");

            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(60)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<CatEstadoHabitacion>(entity =>
        {
            entity.HasKey(e => e.IdEstado).HasName("PK__cat_esta__86989FB271C1A2B1");

            entity.ToTable("cat_estado_habitacion");

            entity.Property(e => e.IdEstado).HasColumnName("id_estado");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<CatEstadoSunat>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__cat_esta__40F9A2078D2CA2FF");

            entity.ToTable("cat_estado_sunat");

            entity.Property(e => e.Codigo)
                .ValueGeneratedNever()
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(60)
                .HasColumnName("descripcion");
            entity.Property(e => e.DescripcionLarga)
                .HasMaxLength(200)
                .HasColumnName("descripcion_larga");
        });

        modelBuilder.Entity<CatMetodoPago>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__cat_meto__40F9A207652E62DF");

            entity.ToTable("cat_metodo_pago");

            entity.Property(e => e.Codigo)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(60)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<CatRolUsuario>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__cat_rol___6ABCB5E0E79101CE");

            entity.ToTable("cat_rol_usuario");

            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.Nombre)
                .HasMaxLength(30)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<CatTipoComprobante>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__cat_tipo__40F9A20765E6F812");

            entity.ToTable("cat_tipo_comprobante");

            entity.Property(e => e.Codigo)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(60)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<CatTipoDocumento>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PK__cat_tipo__40F9A20733CF6DC7");

            entity.ToTable("cat_tipo_documento");

            entity.Property(e => e.Codigo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("codigo");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(60)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdCliente).HasName("PK__clientes__677F38F5EF63D8AF");

            entity.ToTable("clientes");

            entity.HasIndex(e => new { e.TipoDocumento, e.Documento }, "UQ_cliente_documento").IsUnique();

            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.Apellidos)
                .HasMaxLength(100)
                .HasColumnName("apellidos");
            entity.Property(e => e.Direccion)
                .HasMaxLength(200)
                .HasColumnName("direccion");
            entity.Property(e => e.Documento)
                .HasMaxLength(20)
                .HasColumnName("documento");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FechaNacimiento).HasColumnName("fecha_nacimiento");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.FechaVerificacionReniec)
                .HasColumnType("datetime")
                .HasColumnName("fecha_verificacion_reniec");
            entity.Property(e => e.Nacionalidad)
                .HasMaxLength(50)
                .HasDefaultValue("PERUANA")
                .HasColumnName("nacionalidad");
            entity.Property(e => e.Nombres)
                .HasMaxLength(100)
                .HasColumnName("nombres");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .HasColumnName("telefono");
            entity.Property(e => e.TipoDocumento)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tipo_documento");

            entity.HasOne(d => d.TipoDocumentoNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.TipoDocumento)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__clientes__tipo_d__4BAC3F29");
        });

        modelBuilder.Entity<Comprobante>(entity =>
        {
            entity.HasKey(e => e.IdComprobante).HasName("PK__comproba__55E5E24058DF8225");

            entity.ToTable("comprobantes");

            entity.HasIndex(e => new { e.Serie, e.Correlativo }, "UQ__comproba__39CCA08A27E379E3").IsUnique();

            entity.Property(e => e.IdComprobante).HasColumnName("id_comprobante");
            entity.Property(e => e.CdrZip).HasColumnName("cdr_zip");
            entity.Property(e => e.ClienteDocumentoNum)
                .HasMaxLength(20)
                .HasColumnName("cliente_documento_num");
            entity.Property(e => e.ClienteDocumentoTipo)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("cliente_documento_tipo");
            entity.Property(e => e.ClienteNombre)
                .HasMaxLength(200)
                .HasColumnName("cliente_nombre");
            entity.Property(e => e.Correlativo).HasColumnName("correlativo");
            entity.Property(e => e.FechaEmision)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_emision");
            entity.Property(e => e.FechaEnvio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_envio");
            entity.Property(e => e.HashXml)
                .HasMaxLength(64)
                .HasColumnName("hash_xml");
            entity.Property(e => e.IdEstadoSunat)
                .HasDefaultValue(1)
                .HasColumnName("id_estado_sunat");
            entity.Property(e => e.IdEstancia).HasColumnName("id_estancia");
            entity.Property(e => e.IdVenta).HasColumnName("id_venta");
            entity.Property(e => e.IgvMonto)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("igv_monto");
            entity.Property(e => e.IntentosEnvio)
                .HasDefaultValue(0)
                .HasColumnName("intentos_envio");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("metodo_pago");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_total");
            entity.Property(e => e.Serie)
                .HasMaxLength(4)
                .HasColumnName("serie");
            entity.Property(e => e.TipoComprobante)
                .HasMaxLength(2)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("tipo_comprobante");
            entity.Property(e => e.XmlFirmado).HasColumnName("xml_firmado");

            entity.HasOne(d => d.ClienteDocumentoTipoNavigation).WithMany(p => p.Comprobantes)
                .HasForeignKey(d => d.ClienteDocumentoTipo)
                .HasConstraintName("FK__comproban__clien__09A971A2");

            entity.HasOne(d => d.IdEstadoSunatNavigation).WithMany(p => p.Comprobantes)
                .HasForeignKey(d => d.IdEstadoSunat)
                .HasConstraintName("FK__comproban__id_es__0B91BA14");

            entity.HasOne(d => d.MetodoPagoNavigation).WithMany(p => p.Comprobantes)
                .HasForeignKey(d => d.MetodoPago)
                .HasConstraintName("FK__comproban__metod__0A9D95DB");

            entity.HasOne(d => d.TipoComprobanteNavigation).WithMany(p => p.Comprobantes)
                .HasForeignKey(d => d.TipoComprobante)
                .HasConstraintName("FK__comproban__tipo___07C12930");
        });

        modelBuilder.Entity<Estancia>(entity =>
        {
            entity.HasKey(e => e.IdEstancia).HasName("PK__estancia__BF7E6E9933E39CF2");

            entity.ToTable("estancias");

            entity.Property(e => e.IdEstancia).HasColumnName("id_estancia");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Activa")
                .HasColumnName("estado");
            entity.Property(e => e.FechaCheckin)
                .HasColumnType("datetime")
                .HasColumnName("fecha_checkin");
            entity.Property(e => e.FechaCheckoutPrevista)
                .HasColumnType("datetime")
                .HasColumnName("fecha_checkout_prevista");
            entity.Property(e => e.FechaCheckoutReal)
                .HasColumnType("datetime")
                .HasColumnName("fecha_checkout_real");
            entity.Property(e => e.IdClienteTitular).HasColumnName("id_cliente_titular");
            entity.Property(e => e.IdHabitacion).HasColumnName("id_habitacion");
            entity.Property(e => e.IdReserva).HasColumnName("id_reserva");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_total");

            entity.HasOne(d => d.IdClienteTitularNavigation).WithMany(p => p.Estancia)
                .HasForeignKey(d => d.IdClienteTitular)
                .HasConstraintName("FK__estancias__id_cl__6B24EA82");

            entity.HasOne(d => d.IdHabitacionNavigation).WithMany(p => p.Estancia)
                .HasForeignKey(d => d.IdHabitacion)
                .HasConstraintName("FK__estancias__id_ha__6A30C649");

            entity.HasOne(d => d.IdReservaNavigation).WithMany(p => p.Estancia)
                .HasForeignKey(d => d.IdReserva)
                .HasConstraintName("FK__estancias__id_re__693CA210");
        });

        modelBuilder.Entity<Habitacione>(entity =>
        {
            entity.HasKey(e => e.IdHabitacion).HasName("PK__habitaci__773F28F33AE70E23");

            entity.ToTable("habitaciones");

            entity.HasIndex(e => e.NumeroHabitacion, "UQ__habitaci__DC3F4DB4A38C2366").IsUnique();

            entity.Property(e => e.IdHabitacion).HasColumnName("id_habitacion");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.FechaUltimoCambio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_ultimo_cambio");
            entity.Property(e => e.IdEstado)
                .HasDefaultValue(1)
                .HasColumnName("id_estado");
            entity.Property(e => e.IdTipo).HasColumnName("id_tipo");
            entity.Property(e => e.NumeroHabitacion)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("numero_habitacion");
            entity.Property(e => e.Piso)
                .HasDefaultValue(1)
                .HasColumnName("piso");
            entity.Property(e => e.PrecioNoche)
                .HasDefaultValue(50.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_noche");
            entity.Property(e => e.UsuarioCambio).HasColumnName("usuario_cambio");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Habitaciones)
                .HasForeignKey(d => d.IdEstado)
                .HasConstraintName("FK__habitacio__id_es__5812160E");

            entity.HasOne(d => d.IdTipoNavigation).WithMany(p => p.Habitaciones)
                .HasForeignKey(d => d.IdTipo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__habitacio__id_ti__5629CD9C");

            entity.HasOne(d => d.UsuarioCambioNavigation).WithMany(p => p.Habitaciones)
                .HasForeignKey(d => d.UsuarioCambio)
                .HasConstraintName("FK__habitacio__usuar__5AEE82B9");
        });

        modelBuilder.Entity<HistorialEstadoHabitacion>(entity =>
        {
            entity.HasKey(e => e.IdHistorial).HasName("PK__historia__76E6C5029EE1DA34");

            entity.ToTable("historial_estado_habitacion");

            entity.HasIndex(e => new { e.IdHabitacion, e.FechaCambio }, "IX_historial_habitacion_fecha").IsDescending(false, true);

            entity.Property(e => e.IdHistorial).HasColumnName("id_historial");
            entity.Property(e => e.FechaCambio)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_cambio");
            entity.Property(e => e.IdEstadoAnterior).HasColumnName("id_estado_anterior");
            entity.Property(e => e.IdEstadoNuevo).HasColumnName("id_estado_nuevo");
            entity.Property(e => e.IdHabitacion).HasColumnName("id_habitacion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Observacion)
                .HasMaxLength(200)
                .HasColumnName("observacion");

            entity.HasOne(d => d.IdHabitacionNavigation).WithMany(p => p.HistorialEstadoHabitacions)
                .HasForeignKey(d => d.IdHabitacion)
                .HasConstraintName("FK__historial__id_ha__5DCAEF64");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.HistorialEstadoHabitacions)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__historial__id_us__5FB337D6");
        });

        modelBuilder.Entity<Huespede>(entity =>
        {
            entity.HasKey(e => e.IdHuesped).HasName("PK__huespede__55CED1052C95D7A6");

            entity.ToTable("huespedes");

            entity.Property(e => e.IdHuesped).HasColumnName("id_huesped");
            entity.Property(e => e.EsTitular)
                .HasDefaultValue(false)
                .HasColumnName("es_titular");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdEstancia).HasColumnName("id_estancia");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Huespedes)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__huespedes__id_cl__70DDC3D8");

            entity.HasOne(d => d.IdEstanciaNavigation).WithMany(p => p.Huespedes)
                .HasForeignKey(d => d.IdEstancia)
                .HasConstraintName("FK__huespedes__id_es__6FE99F9F");
        });

        modelBuilder.Entity<ItemsVentum>(entity =>
        {
            entity.HasKey(e => e.IdItem).HasName("PK__items_ve__87C9438BDAB67D86");

            entity.ToTable("items_venta");

            entity.Property(e => e.IdItem).HasColumnName("id_item");
            entity.Property(e => e.Cantidad).HasColumnName("cantidad");
            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.IdVenta).HasColumnName("id_venta");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.Subtotal)
                .HasComputedColumnSql("([cantidad]*[precio_unitario])", true)
                .HasColumnType("decimal(21, 2)")
                .HasColumnName("subtotal");

            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.ItemsVenta)
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK__items_ven__id_pr__03F0984C");

            entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.ItemsVenta)
                .HasForeignKey(d => d.IdVenta)
                .HasConstraintName("FK__items_ven__id_ve__02FC7413");
        });

        modelBuilder.Entity<Producto>(entity =>
        {
            entity.HasKey(e => e.IdProducto).HasName("PK__producto__FF341C0D3B9EBE59");

            entity.ToTable("productos");

            entity.Property(e => e.IdProducto).HasColumnName("id_producto");
            entity.Property(e => e.CodigoSunat)
                .HasMaxLength(20)
                .HasColumnName("codigo_sunat");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.IdAfectacionIgv)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasDefaultValue("10")
                .IsFixedLength()
                .HasColumnName("id_afectacion_igv");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
            entity.Property(e => e.PrecioUnitario)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_unitario");
            entity.Property(e => e.Stock)
                .HasDefaultValue(0)
                .HasColumnName("stock");
            entity.Property(e => e.StockMinimo)
                .HasDefaultValue(5)
                .HasColumnName("stock_minimo");
            entity.Property(e => e.UnidadMedida)
                .HasMaxLength(3)
                .HasDefaultValue("NIU")
                .HasColumnName("unidad_medida");

            entity.HasOne(d => d.IdAfectacionIgvNavigation).WithMany(p => p.Productos)
                .HasForeignKey(d => d.IdAfectacionIgv)
                .HasConstraintName("FK__productos__id_af__75A278F5");
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.HasKey(e => e.IdReserva).HasName("PK__reservas__423CBE5D5A766492");

            entity.ToTable("reservas");

            entity.Property(e => e.IdReserva).HasColumnName("id_reserva");
            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente")
                .HasColumnName("estado");
            entity.Property(e => e.FechaEntradaPrevista)
                .HasColumnType("datetime")
                .HasColumnName("fecha_entrada_prevista");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.FechaSalidaPrevista)
                .HasColumnType("datetime")
                .HasColumnName("fecha_salida_prevista");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdHabitacion).HasColumnName("id_habitacion");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.MontoTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("monto_total");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(300)
                .HasColumnName("observaciones");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__reservas__id_cli__628FA481");

            entity.HasOne(d => d.IdHabitacionNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdHabitacion)
                .HasConstraintName("FK__reservas__id_hab__6383C8BA");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Reservas)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__reservas__id_usu__6477ECF3");
        });

        modelBuilder.Entity<TiposHabitacion>(entity =>
        {
            entity.HasKey(e => e.IdTipo).HasName("PK__tipos_ha__CF90108977B67ACA");

            entity.ToTable("tipos_habitacion");

            entity.Property(e => e.IdTipo).HasColumnName("id_tipo");
            entity.Property(e => e.Capacidad)
                .HasDefaultValue(2)
                .HasColumnName("capacidad");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(200)
                .HasColumnName("descripcion");
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .HasColumnName("nombre");
            entity.Property(e => e.PrecioBase)
                .HasDefaultValue(50.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_base");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__usuarios__4E3E04AD13CBB22A");

            entity.ToTable("usuarios");

            entity.HasIndex(e => e.Username, "UQ__usuarios__F3DBC5726C7576AC").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.EstaActivo)
                .HasDefaultValue(true)
                .HasColumnName("esta_activo");
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_creacion");
            entity.Property(e => e.IdRol).HasColumnName("id_rol");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__usuarios__id_rol__45F365D3");
        });

        modelBuilder.Entity<VCierreCajaDiario>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_cierre_caja_diario");

            entity.Property(e => e.Concepto)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("concepto");
            entity.Property(e => e.Fecha).HasColumnName("fecha");
            entity.Property(e => e.Ingresos)
                .HasColumnType("decimal(38, 2)")
                .HasColumnName("ingresos");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(60)
                .HasColumnName("metodo_pago");
        });

        modelBuilder.Entity<VEstadoHabitacione>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("v_estado_habitaciones");

            entity.Property(e => e.Estado)
                .HasMaxLength(30)
                .HasColumnName("estado");
            entity.Property(e => e.FechaUltimoCambio)
                .HasColumnType("datetime")
                .HasColumnName("fecha_ultimo_cambio");
            entity.Property(e => e.NumeroHabitacion)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("numero_habitacion");
            entity.Property(e => e.PrecioNoche)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("precio_noche");
            entity.Property(e => e.TipoHabitacion)
                .HasMaxLength(50)
                .HasColumnName("tipo_habitacion");
        });

        modelBuilder.Entity<Venta>(entity =>
        {
            entity.HasKey(e => e.IdVenta).HasName("PK__ventas__459533BFFA0A0785");

            entity.ToTable("ventas");

            entity.Property(e => e.IdVenta).HasColumnName("id_venta");
            entity.Property(e => e.FechaVenta)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_venta");
            entity.Property(e => e.IdCliente).HasColumnName("id_cliente");
            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.MetodoPago)
                .HasMaxLength(3)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("metodo_pago");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");

            entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdCliente)
                .HasConstraintName("FK__ventas__id_clien__7D439ABD");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK__ventas__id_usuar__7E37BEF6");

            entity.HasOne(d => d.MetodoPagoNavigation).WithMany(p => p.Venta)
                .HasForeignKey(d => d.MetodoPago)
                .HasConstraintName("FK__ventas__metodo_p__00200768");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
