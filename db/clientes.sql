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

-- Crear la tabla DatosClimaticos si no existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE name = 'DatosClimaticos' AND type = 'U')
BEGIN
    CREATE TABLE DatosClimaticos (
        id INT PRIMARY KEY IDENTITY(1,1), -- Identificador único para cada registro
        temperatura DECIMAL(5,2) NOT NULL, -- Temperatura en grados Celsius
        humedad DECIMAL(5,2) NOT NULL,      -- Humedad en porcentaje
        presion DECIMAL(7,2) NOT NULL,      -- Presión en hPa
        fecha DATETIME NOT NULL              -- Fecha y hora de la medición
    );
END
GO

-- Insertar algunos registros de prueba solo si la tabla no tiene datos
IF NOT EXISTS (SELECT * FROM DatosClimaticos)
BEGIN
    INSERT INTO DatosClimaticos (temperatura, humedad, presion, fecha)
    VALUES 
        (22.5, 60.0, 1013.25, '2024-11-01 08:00:00'),
        (21.0, 65.5, 1012.75, '2024-11-01 09:00:00'),
        (20.0, 70.0, 1011.50, '2024-11-01 10:00:00'),
        (19.5, 75.0, 1010.00, '2024-11-01 11:00:00');
END
GO


