-- ������������� ��������� ���� ������
USE RestaurantCanteen;

-- �������� ������� "����"
CREATE TABLE Roles (
    RoleID INT IDENTITY(1,1) PRIMARY KEY,
    RoleName VARCHAR(50) NOT NULL
);

-- ���������� ������� "����" ���������� �������
INSERT INTO Roles (RoleName) VALUES ('admin'), ('cashier'), ('cook'), ('head_cook');

-- �������� ������� "������������"
CREATE TABLE Users (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FullName VARCHAR(255) NOT NULL,
    Login VARCHAR(50) UNIQUE NOT NULL,
    Password VARCHAR(50) NOT NULL,
    Phone VARCHAR(20) NOT NULL,
    RoleID INT NOT NULL,
    FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
);

-- �������� ������� "����� �����������"
CREATE TABLE EmployeeShifts (
    ShiftID INT IDENTITY(1,1) PRIMARY KEY,
    UserID INT,
    ShiftStart DATETIME NOT NULL,
    ShiftEnd DATETIME NOT NULL,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);

-- �������� ������� "�����"
CREATE TABLE Dishes (
    DishID INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(100) NOT NULL,
    Ingredients VARCHAR(255) NOT NULL,
    Price DECIMAL(8, 2) NOT NULL,
    Quantity INT NOT NULL,
    Weight DECIMAL(6, 2) NOT NULL
);

-- �������� ������� "�����"
CREATE TABLE Orders (
    OrderID INT IDENTITY(1,1) PRIMARY KEY,
    CashierID INT,
    OrderTime DATETIME NOT NULL,
    FOREIGN KEY (CashierID) REFERENCES Users(UserID)
);

-- �������� ������� "������ ������"
CREATE TABLE OrderDetails (
    OrderDetailID INT IDENTITY(1,1) PRIMARY KEY,
    OrderID INT,
    DishID INT,
    Quantity INT NOT NULL,
    FOREIGN KEY (OrderID) REFERENCES Orders(OrderID),
    FOREIGN KEY (DishID) REFERENCES Dishes(DishID)
);
