using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using TvControl.Player.App.Model;

namespace TvControl.Player.App.Migrations
{
    [DbContext(typeof(LocalTasksServiceDecorator.TvControlContext))]
    partial class TvControlContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752");

            modelBuilder.Entity("TvControl.Player.App.Model.LocalTasksServiceDecorator+TvControlTaskDbo", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<int>("Position");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });
        }
    }
}
