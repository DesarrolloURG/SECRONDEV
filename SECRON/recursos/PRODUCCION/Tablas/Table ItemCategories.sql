-- Tabla de Articulos
CREATE TABLE ItemCategories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    CategoryCode VARCHAR(10) NOT NULL UNIQUE,
    CategoryName VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    
    CONSTRAINT FK_ItemCategories_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId)
);