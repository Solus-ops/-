CREATE DATABASE ObuvDB;
GO
USE ObuvDB;
GO
CREATE TABLE Stg_Tovar (
  Артикул NVARCHAR(50),
  Наименование NVARCHAR(200),
  Единица NVARCHAR(50),
  Цена NVARCHAR(50),
  Поставщик NVARCHAR(200),
  Производитель NVARCHAR(200),
  Категория NVARCHAR(200),
  Скидка NVARCHAR(50),
  Колво NVARCHAR(50),
  Описание NVARCHAR(MAX),
  Фото NVARCHAR(200)
);

CREATE TABLE Stg_Users (
  Роль NVARCHAR(100),
  ФИО NVARCHAR(200),
  Логин NVARCHAR(100),
  Пароль NVARCHAR(100)
);

CREATE TABLE Stg_Orders (
  Номер NVARCHAR(50),
  Артикулы NVARCHAR(MAX),
  ДатаЗаказа NVARCHAR(50),
  ДатаДоставки NVARCHAR(50),
  Пункт NVARCHAR(200),
  Клиент NVARCHAR(200),
  Код NVARCHAR(50),
  Статус NVARCHAR(100)
);

CREATE TABLE Stg_PickupPoints (
  Address NVARCHAR(500)
);
GO
CREATE TABLE Units (Id INT IDENTITY PRIMARY KEY, Name NVARCHAR(100) UNIQUE);
CREATE TABLE Suppliers (Id INT IDENTITY PRIMARY KEY, Name NVARCHAR(200) UNIQUE);
CREATE TABLE Manufacturers (Id INT IDENTITY PRIMARY KEY, Name NVARCHAR(200) UNIQUE);
CREATE TABLE Categories (Id INT IDENTITY PRIMARY KEY, Name NVARCHAR(200) UNIQUE);
CREATE TABLE Promotions (Id INT IDENTITY PRIMARY KEY, DiscountPercent INT);

CREATE TABLE Roles (Id INT IDENTITY PRIMARY KEY, Name NVARCHAR(200));
CREATE TABLE OrderStatus (Id INT IDENTITY PRIMARY KEY, Name NVARCHAR(200));

CREATE TABLE PickupPoints (Id INT IDENTITY PRIMARY KEY, Address NVARCHAR(500) UNIQUE);

CREATE TABLE Clients (Id INT IDENTITY PRIMARY KEY, FullName NVARCHAR(300));

CREATE TABLE Products (
  Id INT IDENTITY PRIMARY KEY,
  Article NVARCHAR(50),
  Name NVARCHAR(200),
  UnitId INT,
  Price DECIMAL(18,2),
  SupplierId INT,
  ManufacturerId INT,
  CategoryId INT,
  PromotionId INT,
  StockQty INT,
  Description NVARCHAR(MAX),
  Photo NVARCHAR(100),
  FOREIGN KEY (UnitId) REFERENCES Units(Id),
  FOREIGN KEY (SupplierId) REFERENCES Suppliers(Id),
  FOREIGN KEY (ManufacturerId) REFERENCES Manufacturers(Id),
  FOREIGN KEY (CategoryId) REFERENCES Categories(Id),
  FOREIGN KEY (PromotionId) REFERENCES Promotions(Id)
);

CREATE TABLE Users (
  Id INT IDENTITY PRIMARY KEY,
  FullName NVARCHAR(200),
  Login NVARCHAR(100),
  Password NVARCHAR(100),
  RoleId INT,
  FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);

CREATE TABLE Orders (
  Id INT IDENTITY PRIMARY KEY,
  OrderNumber NVARCHAR(50),
  OrderDate DATE,
  DeliveryDate DATE,
  PickupPointId INT,
  ClientId INT,
  PickupCode NVARCHAR(50),
  StatusId INT,
  FOREIGN KEY (PickupPointId) REFERENCES PickupPoints(Id),
  FOREIGN KEY (ClientId) REFERENCES Clients(Id),
  FOREIGN KEY (StatusId) REFERENCES OrderStatus(Id)
);

CREATE TABLE OrderItems (
  Id INT IDENTITY PRIMARY KEY,
  OrderId INT,
  ProductId INT NULL,
  ProductArticle NVARCHAR(50),
  Quantity INT,
  FOREIGN KEY (OrderId) REFERENCES Orders(Id),
  FOREIGN KEY (ProductId) REFERENCES Products(Id)
);
GO
INSERT INTO Units(Name) SELECT DISTINCT Единица FROM Stg_Tovar;

INSERT INTO Suppliers(Name) SELECT DISTINCT Поставщик FROM Stg_Tovar;

INSERT INTO Manufacturers(Name) SELECT DISTINCT Производитель FROM Stg_Tovar;

INSERT INTO Categories(Name) SELECT DISTINCT Категория FROM Stg_Tovar;

INSERT INTO Promotions(DiscountPercent)
SELECT DISTINCT TRY_CAST(Скидка AS INT) FROM Stg_Tovar;

INSERT INTO Roles(Name) SELECT DISTINCT Роль FROM Stg_Users;

INSERT INTO OrderStatus(Name) SELECT DISTINCT Статус FROM Stg_Orders;

INSERT INTO PickupPoints(Address) SELECT DISTINCT Address FROM Stg_PickupPoints;

INSERT INTO Clients(FullName) SELECT DISTINCT Клиент FROM Stg_Orders;
GO
INSERT INTO Products (Article, Name, UnitId, Price, SupplierId, ManufacturerId, CategoryId, PromotionId, StockQty, Description, Photo)
SELECT
  Артикул,
  Наименование,
  u.Id,
  TRY_CAST(Цена AS DECIMAL(18,2)),
  sup.Id,
  man.Id,
  cat.Id,
  prom.Id,
  TRY_CAST(Колво AS INT),
  Описание,
  Фото
FROM Stg_Tovar s
JOIN Units u ON u.Name = s.Единица
JOIN Suppliers sup ON sup.Name = s.Поставщик
JOIN Manufacturers man ON man.Name = s.Производитель
JOIN Categories cat ON cat.Name = s.Категория
JOIN Promotions prom ON prom.DiscountPercent = TRY_CAST(s.Скидка AS INT);
GO
INSERT INTO Users (FullName, Login, Password, RoleId)
SELECT s.ФИО, s.Логин, s.Пароль, r.Id
FROM Stg_Users s
JOIN Roles r ON r.Name = s.Роль;
GO
INSERT INTO Orders (OrderNumber, OrderDate, DeliveryDate, PickupPointId, ClientId, PickupCode, StatusId)
SELECT 
  Номер,
  TRY_CONVERT(date, ДатаЗаказа, 104),
  TRY_CONVERT(date, ДатаДоставки, 104),
  pp.Id,
  cl.Id,
  Код,
  st.Id
FROM Stg_Orders s
JOIN PickupPoints pp ON s.Пункт = pp.Id
JOIN Clients cl ON cl.FullName = s.Клиент
JOIN OrderStatus st ON st.Name = s.Статус;
GO
IF OBJECT_ID('tempdb..#RawOrderItems') IS NOT NULL DROP TABLE #RawOrderItems;

CREATE TABLE #RawOrderItems (
    OrderId INT,
    Token NVARCHAR(100),
    Seq INT
);
GO
DECLARE 
    @OrderId INT,
    @Str NVARCHAR(MAX);

DECLARE order_cursor CURSOR FOR
SELECT o.Id, s.Артикулы
FROM Orders o
JOIN Stg_Orders s ON o.OrderNumber = s.Номер;

OPEN order_cursor;
FETCH NEXT FROM order_cursor INTO @OrderId, @Str;

WHILE @@FETCH_STATUS = 0
BEGIN
    ;WITH CTE AS (
        SELECT 
            @OrderId AS OrderId,
            value AS Token,
            ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) AS Seq
        FROM STRING_SPLIT(REPLACE(REPLACE(@Str, ' ', ''), ';', ','), ',')
        WHERE value <> ''
    )
    INSERT INTO #RawOrderItems (OrderId, Token, Seq)
    SELECT OrderId, Token, Seq FROM CTE;

    FETCH NEXT FROM order_cursor INTO @OrderId, @Str;
END

CLOSE order_cursor;
DEALLOCATE order_cursor;
GO
INSERT INTO OrderItems (OrderId, ProductId, ProductArticle, Quantity)
SELECT 
    r1.OrderId,
    p.Id AS ProductId,
    r1.Token AS ProductArticle,
    TRY_CAST(r2.Token AS INT) AS Quantity
FROM #RawOrderItems r1
JOIN #RawOrderItems r2 
    ON r1.OrderId = r2.OrderId
   AND r2.Seq = r1.Seq + 1
LEFT JOIN Products p 
    ON p.Article = r1.Token
WHERE r1.Seq % 2 = 1;
GO