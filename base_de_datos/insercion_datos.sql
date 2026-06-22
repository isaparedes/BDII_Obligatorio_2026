-- INSERCIÓN DE DATOS BÁSICOS
USE CD_Grupo6;

-- ESTADIOS
INSERT INTO
    estadio (
        nombre_estadio,
        pais_estadio,
        ciudad_estadio,
        calle_estadio,
        numero_estadio
    )
VALUES
    -- Estados Unidos
    (
        'MetLife Stadium',
        'Estados Unidos',
        'East Rutherford',
        'MetLife Stadium Dr',
        1
    ),
    (
        'AT&T Stadium',
        'Estados Unidos',
        'Arlington',
        'AT&T Way',
        1
    ),
    (
        'SoFi Stadium',
        'Estados Unidos',
        'Inglewood',
        'Stadium Dr',
        1001
    ),
    (
        'Levi''s Stadium',
        'Estados Unidos',
        'Santa Clara',
        'Marie P DeBartolo Way',
        4900
    ),
    (
        'Lumen Field',
        'Estados Unidos',
        'Seattle',
        'Occidental Ave S',
        800
    ),
    (
        'Mercedes-Benz Stadium',
        'Estados Unidos',
        'Atlanta',
        'AMB Dr NW',
        1
    ),
    (
        'NRG Stadium',
        'Estados Unidos',
        'Houston',
        'NRG Pkwy',
        1
    ),
    (
        'Arrowhead Stadium',
        'Estados Unidos',
        'Kansas City',
        'Arrowhead Dr',
        1
    ),
    (
        'Lincoln Financial Field',
        'Estados Unidos',
        'Philadelphia',
        'Lincoln Financial Field Way',
        1
    ),
    (
        'Hard Rock Stadium',
        'Estados Unidos',
        'Miami Gardens',
        'Don Shula Dr',
        347
    ),
    (
        'Gillette Stadium',
        'Estados Unidos',
        'Foxborough',
        'Patriot Pl',
        1
    ),
    -- México
    (
        'Estadio Azteca',
        'México',
        'Ciudad de México',
        'Calzada de Tlalpan',
        3465
    ),
    (
        'Estadio BBVA',
        'México',
        'Guadalupe',
        'Av. Pablo Livas',
        2011
    ),
    (
        'Estadio Akron',
        'México',
        'Zapopan',
        'Circuito JVC',
        2800
    ),
    -- Canadá
    (
        'BMO Field',
        'Canadá',
        'Toronto',
        'Princes'' Blvd',
        170
    ),
    (
        'BC Place',
        'Canadá',
        'Vancouver',
        'Pacific Blvd',
        777
    );

-- SECTORES
INSERT INTO
    sector (
        id_estadio,
        nombre_sector,
        costo_sector,
        capacidad
    )
VALUES
    -- 1. MetLife Stadium (Capacidad real: 82.500)
    (1, 'A', 550.00, 8250),
    (1, 'B', 300.00, 16500),
    (1, 'C', 150.00, 24750),
    (1, 'D', 75.00, 33000),
    -- 2. AT&T Stadium (Capacidad real: 80.000)
    (2, 'A', 550.00, 8000),
    (2, 'B', 300.00, 16000),
    (2, 'C', 150.00, 24000),
    (2, 'D', 75.00, 32000),
    -- 3. SoFi Stadium (Capacidad real: 70.000)
    (3, 'A', 550.00, 7000),
    (3, 'B', 300.00, 14000),
    (3, 'C', 150.00, 21000),
    (3, 'D', 75.00, 28000),
    -- 4. Levi's Stadium (Capacidad real: 68.500)
    (4, 'A', 550.00, 6850),
    (4, 'B', 300.00, 13700),
    (4, 'C', 150.00, 20550),
    (4, 'D', 75.00, 27400),
    -- 5. Lumen Field (Capacidad real: 69.000)
    (5, 'A', 550.00, 6900),
    (5, 'B', 300.00, 13800),
    (5, 'C', 150.00, 20700),
    (5, 'D', 75.00, 27600),
    -- 6. Mercedes-Benz Stadium (Capacidad real: 71.000)
    (6, 'A', 550.00, 7100),
    (6, 'B', 300.00, 14200),
    (6, 'C', 150.00, 21300),
    (6, 'D', 75.00, 28400),
    -- 7. NRG Stadium (Capacidad real: 72.000)
    (7, 'A', 550.00, 7200),
    (7, 'B', 300.00, 14400),
    (7, 'C', 150.00, 21600),
    (7, 'D', 75.00, 28800),
    -- 8. Arrowhead Stadium (Capacidad real: 76.400)
    (8, 'A', 550.00, 7640),
    (8, 'B', 300.00, 15280),
    (8, 'C', 150.00, 22920),
    (8, 'D', 75.00, 30560),
    -- 9. Lincoln Financial Field (Capacidad real: 69.500)
    (9, 'A', 550.00, 6950),
    (9, 'B', 300.00, 13900),
    (9, 'C', 150.00, 20850),
    (9, 'D', 75.00, 27800),
    -- 10. Hard Rock Stadium (Capacidad real: 65.000)
    (10, 'A', 550.00, 6500),
    (10, 'B', 300.00, 13000),
    (10, 'C', 150.00, 19500),
    (10, 'D', 75.00, 26000),
    -- 11. Gillette Stadium (Capacidad real: 65.800)
    (11, 'A', 550.00, 6580),
    (11, 'B', 300.00, 13160),
    (11, 'C', 150.00, 19740),
    (11, 'D', 75.00, 26320),
    -- 12. Estadio Azteca (Capacidad real: 87.500)
    (12, 'A', 550.00, 8750),
    (12, 'B', 300.00, 17500),
    (12, 'C', 150.00, 26250),
    (12, 'D', 75.00, 35000),
    -- 13. Estadio BBVA (Capacidad real: 53.500)
    (13, 'A', 550.00, 5350),
    (13, 'B', 300.00, 10700),
    (13, 'C', 150.00, 16050),
    (13, 'D', 75.00, 21400),
    -- 14. Estadio Akron (Capacidad real: 49.850)
    (14, 'A', 550.00, 4985),
    (14, 'B', 300.00, 9970),
    (14, 'C', 150.00, 14955),
    (14, 'D', 75.00, 19940),
    -- 15. BMO Field (Capacidad real: 45.000)
    (15, 'A', 550.00, 4500),
    (15, 'B', 300.00, 9000),
    (15, 'C', 150.00, 13500),
    (15, 'D', 75.00, 18000),
    -- 16. BC Place (Capacidad real: 54.500)
    (16, 'A', 550.00, 5450),
    (16, 'B', 300.00, 10900),
    (16, 'C', 150.00, 16350),
    (16, 'D', 75.00, 21800);

-- EQUIPOS
INSERT INTO
    equipo (nombre_equipo)
VALUES
    -- GRUPO A
    ('México'),
    ('Corea del Sur'),
    ('Chequia'),
    ('Sudáfrica'),
    -- GRUPO B
    ('Suiza'),
    ('Canadá'),
    ('Catar'),
    ('Bosnia y Herzegovina'),
    -- GRUPO C
    ('Escocia'),
    ('Marruecos'),
    ('Brasil'),
    ('Haití'),
    -- GRUPO D
    ('Estados Unidos'),
    ('Australia'),
    ('Turquía'),
    ('Paraguay'),
    -- GRUPO E
    ('Alemania'),
    ('Costa de Marfil'),
    ('Ecuador'),
    ('Curazao'),
    -- GRUPO F
    ('Suecia'),
    ('Japón'),
    ('Países Bajos'),
    ('Túnez'),
    -- GRUPO G
    ('Nueva Zelanda'),
    ('Irán'),
    ('Bélgica'),
    ('Egipto'),
    -- GRUPO H
    ('Uruguay'),
    ('Arabia Saudita'),
    ('España'),
    ('Cabo Verde'),
    -- GRUPO I
    ('Francia'),
    ('Senegal'),
    ('Irak'),
    ('Noruega'),
    -- GRUPO J
    ('Argentina'),
    ('Argelia'),
    ('Austria'),
    ('Jordania'),
    -- GRUPO K
    ('Portugal'),
    ('RD Congo'),
    ('Uzbekistán'),
    ('Colombia'),
    -- GRUPO L
    ('Inglaterra'),
    ('Croacia'),
    ('Ghana'),
    ('Panamá');