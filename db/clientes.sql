-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BD_CLIENTES')
BEGIN
    CREATE DATABASE BD_CLIENTES;
END
GO

-- Utilizar la base de datos
USE BD_CLIENTES;
GO

-- Crear la tabla TiposDocumentos si no existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'TiposDocumentos' AND type = 'U')
BEGIN
    CREATE TABLE TiposDocumentos (
        Id INT PRIMARY KEY IDENTITY(1,1), -- Identificador único para cada tipo de documento
        Descripcion NVARCHAR(100) NOT NULL -- Descripción del tipo de documento
    );
END
GO

-- Insertar algunos registros de prueba solo si la tabla no tiene datos
IF NOT EXISTS (SELECT * FROM TiposDocumentos)
BEGIN
    INSERT INTO TiposDocumentos (Descripcion)
    VALUES 
        ('DNI'),
        ('Pasaporte'),
        ('Carnet de Extranjería'),
        ('Licencia de Conducir');
END
GO


