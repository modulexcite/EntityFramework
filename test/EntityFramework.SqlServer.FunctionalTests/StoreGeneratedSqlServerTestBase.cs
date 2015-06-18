// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Data.Entity.FunctionalTests;
using Microsoft.Framework.DependencyInjection;

namespace Microsoft.Data.Entity.SqlServer.FunctionalTests
{
    public class StoreGeneratedSqlServerTest
        : StoreGeneratedTestBase<SqlServerTestStore, StoreGeneratedSqlServerTest.StoreGeneratedSqlServerFixture>
    {
        public StoreGeneratedSqlServerTest(StoreGeneratedSqlServerFixture fixture)
            : base(fixture)
        {
        }

        public class StoreGeneratedSqlServerFixture : StoreGeneratedFixtureBase
        {
            private const string DatabaseName = "StoreGeneratedTest";

            private readonly IServiceProvider _serviceProvider;

            public StoreGeneratedSqlServerFixture()
            {
                _serviceProvider = new ServiceCollection()
                    .AddEntityFramework()
                    .AddSqlServer()
                    .ServiceCollection()
                    .AddSingleton(TestSqlServerModelSource.GetFactory(OnModelCreating))
                    .BuildServiceProvider();
            }

            public override SqlServerTestStore CreateTestStore()
            {
                return SqlServerTestStore.GetOrCreateShared(DatabaseName, () =>
                    {
                        var optionsBuilder = new DbContextOptionsBuilder();
                        optionsBuilder.UseSqlServer(SqlServerTestStore.CreateConnectionString(DatabaseName));

                        using (var context = new StoreGeneratedContext(_serviceProvider, optionsBuilder.Options))
                        {
                            context.Database.EnsureDeleted();
                            context.Database.EnsureCreated();
                        }
                    });
            }

            public override DbContext CreateContext(SqlServerTestStore testStore)
            {
                var optionsBuilder = new DbContextOptionsBuilder();
                optionsBuilder.UseSqlServer(testStore.Connection);

                var context = new StoreGeneratedContext(_serviceProvider, optionsBuilder.Options);
                context.Database.GetRelationalConnection().UseTransaction(testStore.Transaction);

                return context;
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Gumball>(b =>
                    {
                        b.Property(e => e.Id)
                            .ForSqlServer()
                            .UseIdentity();

                        b.Property(e => e.Identity)
                            .DefaultValue("Banana Joe");

                        b.Property(e => e.IdentityReadOnlyBeforeSave)
                            .DefaultValue("Doughnut Sheriff");

                        b.Property(e => e.IdentityReadOnlyAfterSave)
                            .DefaultValue("Anton");

                        b.Property(e => e.Computed)
                            .DefaultValue("Alan");

                        b.Property(e => e.ComputedReadOnlyBeforeSave)
                            .DefaultValue("Carmen");

                        b.Property(e => e.ComputedReadOnlyAfterSave)
                            .DefaultValue("Tina Rex");
                    });
            }
        }
    }
}
