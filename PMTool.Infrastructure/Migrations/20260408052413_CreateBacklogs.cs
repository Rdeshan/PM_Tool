using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateBacklogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No-op: OwnerId was already added by migration 20260408000100_AddBacklogOwner.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op.
        }
    }
}
