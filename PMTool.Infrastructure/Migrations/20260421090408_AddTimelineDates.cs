using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PMTool.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTimelineDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'ProjectBacklogs' AND COLUMN_NAME = 'DueDate')
                BEGIN
                    ALTER TABLE [ProjectBacklogs] ADD [DueDate] datetime2 NULL;
                END");

            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                               WHERE TABLE_NAME = 'ProjectBacklogs' AND COLUMN_NAME = 'StartDate')
                BEGIN
                    ALTER TABLE [ProjectBacklogs] ADD [StartDate] datetime2 NULL;
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                           WHERE TABLE_NAME = 'ProjectBacklogs' AND COLUMN_NAME = 'DueDate')
                BEGIN
                    ALTER TABLE [ProjectBacklogs] DROP COLUMN [DueDate];
                END");

            migrationBuilder.Sql(@"
                IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                           WHERE TABLE_NAME = 'ProjectBacklogs' AND COLUMN_NAME = 'StartDate')
                BEGIN
                    ALTER TABLE [ProjectBacklogs] DROP COLUMN [StartDate];
                END");
        }
    }
}
