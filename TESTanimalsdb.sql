USE MASTER
GO

IF NOT EXISTS (
    SELECT [name]
    FROM sys.databases
    WHERE [name] = N'AnimalsDB'
)
CREATE DATABASE AnimalsDB
GO

USE AnimalsDB
GO

DROP TABLE IF EXISTS Zoos;
DROP TABLE IF EXISTS Animals;

CREATE TABLE Zoos (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    [Name] VARCHAR(50) NOT NULL,
    [Address] VARCHAR(100) NOT NULL,
    Acres DECIMAL(5,2)
);

INSERT INTO Zoos ([Name], [Address], Acres)
VALUES ('Nashville Zoo at Grassmere', '3777 Nolensville Pike, Nashville, TN 37211', 188);

INSERT INTO Zoos ([Name], [Address], Acres)
VALUES ('San Diego', '2920 Zoo Dr, San Diego, CA 92101', 100);

CREATE TABLE Animals (
    Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
    [Name] VARCHAR(50) NOT NULL,
    Species VARCHAR(50) NOT NULL,
    EatingHabit VARCHAR(50),
    Legs INT,
    ZooId INT NOT NULL,
    CONSTRAINT FK_AnimalZoo FOREIGN KEY(ZooId) REFERENCES Zoos(Id)
);

INSERT INTO Animals ([Name], Species, EatingHabit, Legs, ZooId)
VALUES ('Fake Butter', 'Northwest African Cheetah', 'Carnivore', 4, 2);

INSERT INTO Animals ([Name], Species, EatingHabit, Legs, ZooId)
VALUES ('Fake Coconut', 'Northwest African Cheetah', 'Carnivore', 4, 2);

INSERT INTO Animals ([Name], Species, EatingHabit, Legs, ZooId)
VALUES ('Fake Peach', 'Macaroni Penguin', 'Herbivore', 2, 1);

INSERT INTO Animals ([Name], Species, EatingHabit, Legs, ZooId)
VALUES ('Fake Marshmallow', 'Macaroni Penguin', 'Herbivore', 2, 1);

INSERT INTO Animals ([Name], Species, EatingHabit, Legs, ZooId)
VALUES ('Fake Tutti Frutti', 'Spinner Dolphin', 'Carnivore', 0, 2);

INSERT INTO Animals ([Name], Species, EatingHabit, Legs, ZooId)
VALUES ('Fake Bubble Gum', 'Spinner Dolphin', 'Carnivore', 0, 2);

INSERT INTO Animals ([Name], Species, EatingHabit, Legs, ZooId)
VALUES ('Lemon Drop', 'Sea Otter', 'Omnivore', 4, 1);

INSERT INTO Animals ([Name], Species, EatingHabit, Legs, ZooId)
VALUES ('Fake Margarita', 'Sea Otter', 'Omnivore', 4, 1);