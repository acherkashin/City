﻿// <auto-generated />
using CyberCity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace CyberCity.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    partial class ApplicationContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011");

            modelBuilder.Entity("CyberCity.Models.AccountModel.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ArduinoUrl");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("Login");

                    b.Property<string>("Password");

                    b.Property<int>("Subject");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CyberCity.Models.BankModel.Credit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Client");

                    b.Property<int>("Duration");

                    b.Property<double>("Summa");

                    b.HasKey("Id");

                    b.ToTable("Credits");
                });

            modelBuilder.Entity("CyberCity.Models.BankModel.MoneyContribution", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Client");

                    b.Property<int>("Duration");

                    b.Property<double>("Summa");

                    b.HasKey("Id");

                    b.ToTable("MoneyContributions");
                });

            modelBuilder.Entity("CyberCity.Models.BankModel.Moneytransfer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Recipient");

                    b.Property<int>("Sender");

                    b.Property<double>("Summa");

                    b.HasKey("Id");

                    b.ToTable("Moneytransfers");
                });

            modelBuilder.Entity("CyberCity.Models.BankModel.NeedOperation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Money");

                    b.Property<int>("Recipient");

                    b.Property<int>("Sender");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.ToTable("NeedOprerations");
                });

            modelBuilder.Entity("CyberCity.Models.BankModel.PaymentUtilit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Client");

                    b.Property<double>("Summa");

                    b.HasKey("Id");

                    b.ToTable("PaymentUtilits");
                });

            modelBuilder.Entity("CyberCity.Models.BankModel.Resident", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<double>("Debt");

                    b.Property<int>("Home");

                    b.Property<string>("Login")
                        .IsRequired();

                    b.Property<double>("Money");

                    b.Property<double>("MoneyInCourse");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.Property<string>("Password")
                        .IsRequired();

                    b.Property<string>("Patronymic")
                        .IsRequired();

                    b.Property<string>("Surname")
                        .IsRequired();

                    b.HasKey("Id");

                    b.ToTable("Residents");
                });

            modelBuilder.Entity("CyberCity.Models.BankModel.Salary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Client");

                    b.Property<double>("Summa");

                    b.Property<DateTime>("Time");

                    b.HasKey("Id");

                    b.ToTable("Salarys");
                });

            modelBuilder.Entity("CyberCity.Models.Package", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateTime");

                    b.Property<int>("From");

                    b.Property<string>("Method");

                    b.Property<string>("Params");

                    b.Property<int>("To");

                    b.HasKey("Id");

                    b.ToTable("Packages");
                });
#pragma warning restore 612, 618
        }
    }
}
