using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Practice.Models;

namespace Practice.Data
{
    public partial class CourseProject2DBContext : DbContext
    {
        public CourseProject2DBContext()
        {
        }

        public CourseProject2DBContext(DbContextOptions<CourseProject2DBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ВидыТрудоустройства> ВидыТрудоустройстваs { get; set; } = null!;
        public virtual DbSet<Департаменты> Департаментыs { get; set; } = null!;
        public virtual DbSet<Должности> Должностиs { get; set; } = null!;
        public virtual DbSet<ДолжностиСотрудников> ДолжностиСотрудниковs { get; set; } = null!;
        public virtual DbSet<Пол> Полs { get; set; } = null!;
        public virtual DbSet<Проекты> Проектыs { get; set; } = null!;
        public virtual DbSet<ПроектыИСотрудники> ПроектыИСотрудникиs { get; set; } = null!;
        public virtual DbSet<Роли> Ролиs { get; set; } = null!;
        public virtual DbSet<Сотрудники> Сотрудникиs { get; set; } = null!;
        public virtual DbSet<СтавкиСотрудников> СтавкиСотрудниковs { get; set; } = null!;
        public virtual DbSet<Статусы> Статусыs { get; set; } = null!;
        public virtual DbSet<ТипыПроектов> ТипыПроектовs { get; set; } = null!;
        public virtual DbSet<УстройствоНаРаботу> УстройствоНаРаботуs { get; set; } = null!;
        public virtual DbSet<ФактическиеТрудозатраты> ФактическиеТрудозатратыs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CourseProject2DB;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Cyrillic_General_CI_AS");

            modelBuilder.Entity<ВидыТрудоустройства>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Виды_трудоустройства");

                entity.Property(e => e.ВидТрудоустройства)
                    .HasMaxLength(50)
                    .HasColumnName("Вид_трудоустройства")
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            });

            modelBuilder.Entity<Департаменты>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Департаменты");

                entity.Property(e => e.КодДиректораДепартамента).HasColumnName("Код_директора_департамента");

                entity.Property(e => e.НазваниеДепартамента)
                    .HasMaxLength(50)
                    .HasColumnName("Название_департамента")
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");

                entity.HasOne(d => d.КодДиректораДепартаментаNavigation)
                    .WithMany(p => p.Департаментыs)
                    .HasForeignKey(d => d.КодДиректораДепартамента)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Департаменты_Сотрудники");
            });

            modelBuilder.Entity<Должности>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Должности");

                entity.Property(e => e.Должность)
                    .HasMaxLength(50)
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            });

            modelBuilder.Entity<ДолжностиСотрудников>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Должности_сотрудников");

                entity.Property(e => e.ДатаНазначения)
                    .HasColumnType("date")
                    .HasColumnName("Дата_назначения");

                entity.Property(e => e.КодДепартамента).HasColumnName("Код_департамента");

                entity.Property(e => e.КодДолжности).HasColumnName("Код_должности");

                entity.Property(e => e.КодСотрудника).HasColumnName("Код_сотрудника");

                entity.HasOne(d => d.КодДепартаментаNavigation)
                    .WithMany(p => p.ДолжностиСотрудниковs)
                    .HasForeignKey(d => d.КодДепартамента)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Должности_сотрудников_Департаменты");

                entity.HasOne(d => d.КодДолжностиNavigation)
                    .WithMany(p => p.ДолжностиСотрудниковs)
                    .HasForeignKey(d => d.КодДолжности)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Должности_сотрудников_Должности");

                entity.HasOne(d => d.КодСотрудникаNavigation)
                    .WithMany(p => p.ДолжностиСотрудниковs)
                    .HasForeignKey(d => d.КодСотрудника)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Должности_сотрудников_Сотрудники");
            });

            modelBuilder.Entity<Пол>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Пол");

                entity.Property(e => e.Пол1)
                    .HasMaxLength(50)
                    .HasColumnName("Пол")
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            });

            modelBuilder.Entity<Проекты>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Проекты");

                entity.Property(e => e.ДатаЗавершенияПроекта)
                    .HasColumnType("date")
                    .HasColumnName("Дата_завершения_проекта");

                entity.Property(e => e.ДатаНачалаПроекта)
                    .HasColumnType("date")
                    .HasColumnName("Дата_начала_проекта");

                entity.Property(e => e.КодМенеджераПроекта).HasColumnName("Код_менеджера_проекта");

                entity.Property(e => e.КодТипаПроекта).HasColumnName("Код_типа_проекта");

                entity.Property(e => e.НазваниеПроекта)
                    .HasMaxLength(50)
                    .HasColumnName("Название_проекта")
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");

                entity.HasOne(d => d.КодМенеджераПроектаNavigation)
                    .WithMany(p => p.Проектыs)
                    .HasForeignKey(d => d.КодМенеджераПроекта)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Проекты_Сотрудники");

                entity.HasOne(d => d.КодТипаПроектаNavigation)
                    .WithMany(p => p.Проектыs)
                    .HasForeignKey(d => d.КодТипаПроекта)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Проекты_Типы_проектов");
            });

            modelBuilder.Entity<ПроектыИСотрудники>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Проекты_и_сотрудники");

                entity.Property(e => e.ДатаНачалаРаботыНаПроекте)
                    .HasColumnType("date")
                    .HasColumnName("Дата_начала_работы_на_проекте");

                entity.Property(e => e.ДатаОкончанияРаботыНаПроекте)
                    .HasColumnType("date")
                    .HasColumnName("Дата_окончания_работы_на_проекте");

                entity.Property(e => e.КодПроекта).HasColumnName("Код_проекта");

                entity.Property(e => e.КодРоли).HasColumnName("Код_роли");

                entity.Property(e => e.КодСотрудника).HasColumnName("Код_сотрудника");

                entity.HasOne(d => d.КодПроектаNavigation)
                    .WithMany(p => p.ПроектыИСотрудникиs)
                    .HasForeignKey(d => d.КодПроекта)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Проекты_и_сотрудники_Проекты");

                entity.HasOne(d => d.КодРолиNavigation)
                    .WithMany(p => p.ПроектыИСотрудникиs)
                    .HasForeignKey(d => d.КодРоли)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Проекты_и_сотрудники_Роли");

                entity.HasOne(d => d.КодСотрудникаNavigation)
                    .WithMany(p => p.ПроектыИСотрудникиs)
                    .HasForeignKey(d => d.КодСотрудника)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Проекты_и_сотрудники_Сотрудники");
            });

            modelBuilder.Entity<Роли>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Роли");

                entity.Property(e => e.Роль)
                    .HasMaxLength(50)
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            });

            modelBuilder.Entity<Сотрудники>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Сотрудники");

                entity.Property(e => e.ДатаНачалаРаботыВSap)
                    .HasColumnType("date")
                    .HasColumnName("Дата_начала_работы_в_SAP");

                entity.Property(e => e.ДатаРождения)
                    .HasColumnType("date")
                    .HasColumnName("Дата_рождения");

                entity.Property(e => e.Имя)
                    .HasMaxLength(50)
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.КодВидаТрудоустройства).HasColumnName("Код_вида_трудоустройства");

                entity.Property(e => e.КодПола).HasColumnName("Код_пола");

                entity.Property(e => e.Логин)
                    .HasMaxLength(50)
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.Отчество)
                    .HasMaxLength(50)
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.Пароль)
                    .HasMaxLength(100)
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.Телефон1)
                    .HasMaxLength(50)
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.Телефон2)
                    .HasMaxLength(50)
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.Фамилия)
                    .HasMaxLength(50)
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");

                entity.HasOne(d => d.КодВидаТрудоустройстваNavigation)
                    .WithMany(p => p.Сотрудникиs)
                    .HasForeignKey(d => d.КодВидаТрудоустройства)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Сотрудники_Виды_трудоустройства");

                entity.HasOne(d => d.КодПолаNavigation)
                    .WithMany(p => p.Сотрудникиs)
                    .HasForeignKey(d => d.КодПола)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Сотрудники_Пол");
            });

            modelBuilder.Entity<СтавкиСотрудников>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Ставки_сотрудников");

                entity.Property(e => e.ДатаНачалаДействияСтавки)
                    .HasColumnType("date")
                    .HasColumnName("Дата_начала_действия_ставки");

                entity.Property(e => e.КодПроекта).HasColumnName("Код_проекта");

                entity.Property(e => e.КодСотрудника).HasColumnName("Код_сотрудника");

                entity.HasOne(d => d.КодПроектаNavigation)
                    .WithMany(p => p.СтавкиСотрудниковs)
                    .HasForeignKey(d => d.КодПроекта)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ставки_сотрудников_Проекты");

                entity.HasOne(d => d.КодСотрудникаNavigation)
                    .WithMany(p => p.СтавкиСотрудниковs)
                    .HasForeignKey(d => d.КодСотрудника)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Ставки_сотрудников_Сотрудники");
            });

            modelBuilder.Entity<Статусы>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Статусы");

                entity.Property(e => e.Статус)
                    .HasMaxLength(50)
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            });

            modelBuilder.Entity<ТипыПроектов>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Типы_проектов");

                entity.Property(e => e.ТипПроекта)
                    .HasMaxLength(50)
                    .HasColumnName("Тип_проекта")
                    .UseCollation("SQL_Latin1_General_CP1_CI_AS");
            });

            modelBuilder.Entity<УстройствоНаРаботу>(entity =>
            {
                entity.HasKey(e => e.КодУстройстваНаРаботу);

                entity.ToTable("Устройство_на_работу");

                entity.Property(e => e.КодУстройстваНаРаботу).HasColumnName("Код_устройства_на_работу");

                entity.Property(e => e.ДатаЗачисленияВШтат)
                    .HasColumnType("date")
                    .HasColumnName("Дата_зачисления_в_штат");

                entity.Property(e => e.ДатаУвольнения)
                    .HasColumnType("date")
                    .HasColumnName("Дата_увольнения");

                entity.Property(e => e.КодСотрудника).HasColumnName("Код_сотрудника");

                entity.HasOne(d => d.КодСотрудникаNavigation)
                    .WithMany(p => p.УстройствоНаРаботуs)
                    .HasForeignKey(d => d.КодСотрудника)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Устройство_на_работу_Сотрудники");
            });

            modelBuilder.Entity<ФактическиеТрудозатраты>(entity =>
            {
                entity.HasKey(e => e.Код);

                entity.ToTable("Фактические_трудозатраты");

                entity.Property(e => e.ДатаТрудозатраты)
                    .HasColumnType("date")
                    .HasColumnName("Дата_трудозатраты");

                entity.Property(e => e.Задача).UseCollation("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.КодПроекта).HasColumnName("Код_проекта");

                entity.Property(e => e.КодРазработчика).HasColumnName("Код_разработчика");

                entity.Property(e => e.КодСтатуса).HasColumnName("Код_статуса");

                entity.Property(e => e.КоличествоЧасов).HasColumnName("Количество_часов");

                entity.Property(e => e.Комментарий).UseCollation("SQL_Latin1_General_CP1_CI_AS");

                entity.Property(e => e.ПоследнееИзменение)
                    .HasColumnType("datetime")
                    .HasColumnName("Последнее_изменение");

                entity.HasOne(d => d.КодПроектаNavigation)
                    .WithMany(p => p.ФактическиеТрудозатратыs)
                    .HasForeignKey(d => d.КодПроекта)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Фактические_трудозатраты_Проекты");

                entity.HasOne(d => d.КодРазработчикаNavigation)
                    .WithMany(p => p.ФактическиеТрудозатратыs)
                    .HasForeignKey(d => d.КодРазработчика)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Фактические_трудозатраты_Сотрудники");

                entity.HasOne(d => d.КодСтатусаNavigation)
                    .WithMany(p => p.ФактическиеТрудозатратыs)
                    .HasForeignKey(d => d.КодСтатуса)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Фактические_трудозатраты_Статусы");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
