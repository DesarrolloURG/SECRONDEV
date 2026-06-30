CREATE TABLE ItemSubCategories (
    SubCategoryId   INT IDENTITY(1,1) NOT NULL,
    CategoryId      INT NOT NULL,
    SubCategoryCode VARCHAR(10) NOT NULL,
    SubCategoryName VARCHAR(200) NOT NULL,
    IsActive        BIT NOT NULL DEFAULT 1,
    CreatedDate     DATETIME NULL DEFAULT GETDATE(),
    CreatedBy       INT NULL,
    ModifiedDate    DATETIME NULL,
    ModifiedBy      INT NULL,
    CONSTRAINT PK_ItemSubCategories PRIMARY KEY (SubCategoryId),
    CONSTRAINT UK_ISC_Code UNIQUE (CategoryId, SubCategoryCode),
    CONSTRAINT FK_ISC_Category FOREIGN KEY (CategoryId) REFERENCES ItemCategories(CategoryId),
    CONSTRAINT FK_ISC_CreatedBy FOREIGN KEY (CreatedBy) REFERENCES Users(UserId),
    CONSTRAINT FK_ISC_ModifiedBy FOREIGN KEY (ModifiedBy) REFERENCES Users(UserId)
);

ALTER TABLE Items
ADD SubCategoryId INT NULL
CONSTRAINT FK_Items_SubCategory FOREIGN KEY (SubCategoryId)
    REFERENCES ItemSubCategories(SubCategoryId);

CREATE INDEX IX_Items_SubCategory ON Items(SubCategoryId);
