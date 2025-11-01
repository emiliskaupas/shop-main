-- Check if RefreshTokens table exists and create if it doesn't
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RefreshTokens')
BEGIN
    CREATE TABLE [RefreshTokens] (
        [Id] bigint NOT NULL IDENTITY,
        [Token] nvarchar(max) NOT NULL,
        [UserId] bigint NOT NULL,
        [ExpiresAt] datetime2 NOT NULL,
        [IsRevoked] bit NOT NULL,
        [RevokedAt] datetime2 NULL,
        CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );

    CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
    
    PRINT 'RefreshTokens table created successfully';
END
ELSE
BEGIN
    PRINT 'RefreshTokens table already exists';
END

-- Update migration history to mark all migrations as applied
IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20250930185602_InitialCreate')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ('20250930185602_InitialCreate', '8.0.10');
END

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20251006120039_AddReviews')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ('20251006120039_AddReviews', '8.0.10');
END

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20251006133337_AddReviewsTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ('20251006133337_AddReviewsTable', '8.0.10');
END

IF NOT EXISTS (SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20251101135903_AddRefreshTokens')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ('20251101135903_AddRefreshTokens', '8.0.10');
END

PRINT 'Migration history updated';
