-- Plantillas de Stock por Categoría de Sede
CREATE TABLE ItemStockTemplates (
    TemplateId INT IDENTITY(1,1) PRIMARY KEY,
    LocationCategoryId INT NOT NULL,
    ItemId INT NOT NULL,
    MinimumStock DECIMAL(18,2) NOT NULL DEFAULT 0,
    MaximumStock DECIMAL(18,2) NOT NULL DEFAULT 0,
    ReorderPoint DECIMAL(18,2),
    IsActive BIT DEFAULT 1,
    CreatedDate DATETIME DEFAULT GETDATE(),
    CreatedBy INT,
    ModifiedDate DATETIME,
    ModifiedBy INT,
    
    CONSTRAINT FK_StockTemplate_Category FOREIGN KEY (LocationCategoryId) 
        REFERENCES LocationCategories(LocationCategoryId),
    CONSTRAINT FK_StockTemplate_Item FOREIGN KEY (ItemId) 
        REFERENCES Items(ItemId),
    CONSTRAINT FK_StockTemplate_CreatedBy FOREIGN KEY (CreatedBy) 
        REFERENCES Users(UserId),
    CONSTRAINT FK_StockTemplate_ModifiedBy FOREIGN KEY (ModifiedBy) 
        REFERENCES Users(UserId),
    CONSTRAINT UK_Template_Item_Category UNIQUE (LocationCategoryId, ItemId)
);

SELECT * FROM ItemStockTemplates