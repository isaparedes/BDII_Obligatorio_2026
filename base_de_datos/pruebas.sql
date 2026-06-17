-- =====================================================
--  DATOS DE PRUEBA — versión corregida
-- =====================================================
USE CD_Grupo6;

-- Prerrequisitos minimos para evitar fallos por FK cuando
-- insercion_datos.sql se corto por duplicados.
INSERT IGNORE INTO equipo (nombre_equipo) VALUES
('México'),
('Brasil'),
('Uruguay'),
('Argentina'),
('España'),
('Francia');

INSERT IGNORE INTO sector (id_estadio, nombre_sector, costo_sector, capacidad) VALUES
(1, 'A', 550.00, 8250),
(2, 'B', 300.00, 16000),
(3, 'C', 150.00, 21000);

-- ─────────────────────────────────────────────────
--  LIMPIEZA (orden inverso a FK)
-- ─────────────────────────────────────────────────
DELETE FROM token       WHERE id_entrada IN (9001, 9002, 9003);
DELETE FROM asignacion  WHERE id_evento  IN (9001, 9002, 9003);
DELETE FROM entrada     WHERE id_entrada IN (9001, 9002, 9003);
DELETE FROM habilita    WHERE id_evento  IN (9001, 9002, 9003);
DELETE FROM evento      WHERE id_evento  IN (9001, 9002, 9003);
DELETE FROM compra      WHERE id_compra  = 9001;
DELETE FROM comision    WHERE id_comision = 9001;
DELETE FROM dispositivo WHERE mail_funcionario = 'funcionario@demo.com';
DELETE FROM asignacion  WHERE mail_funcionario = 'funcionario@demo.com';
DELETE FROM usuario_general WHERE mail = 'user@test.com';
DELETE FROM funcionario     WHERE mail = 'funcionario@demo.com';
DELETE FROM administrador   WHERE mail = 'admin@demo.com';
DELETE FROM usuario WHERE mail IN ('admin@demo.com','funcionario@demo.com','user@test.com');

-- ─────────────────────────────────────────────────
--  USUARIOS
-- ─────────────────────────────────────────────────
INSERT INTO usuario (mail, contrasena, pais_documento, tipo_documento, numero_documento, pais_direccion, localidad, calle, numero_calle, codigo_postal) VALUES
('admin@demo.com',        'hash_admin', 'Uruguay', 'CI', '11111111', 'Uruguay', 'Montevideo', 'Av. 18 de Julio', 1234, '11100'),
('funcionario@demo.com',  'hash_func',  'Uruguay', 'CI', '22222222', 'Uruguay', 'Montevideo', 'Colonia',         500,  '11100'),
('user@test.com',         'hash_user',  'Uruguay', 'CI', '33333333', 'Uruguay', 'Montevideo', 'Rivera',          800,  '11200');

INSERT INTO administrador  (mail, fecha_asignacion, pais_sede)   VALUES ('admin@demo.com',       '2025-01-01', 'Estados Unidos');
INSERT INTO funcionario    (mail, numero_legajo)                  VALUES ('funcionario@demo.com', 1001);
INSERT INTO dispositivo    (id_dispositivo, mail_funcionario)     VALUES (1, 'funcionario@demo.com');
INSERT INTO usuario_general(mail, estado_verificacion, fecha_registro) VALUES ('user@test.com', 'Aprobado', '2025-01-01');

-- ─────────────────────────────────────────────────
--  COMISIÓN Y COMPRA
-- ─────────────────────────────────────────────────
INSERT INTO comision (id_comision, valor_comision, fecha_inicio, fecha_fin)
VALUES (9001, 5.00, '2026-01-01', '2026-12-31');

INSERT INTO compra (id_compra, fecha_compra, estado_compra, monto_total, mail_comprador, id_comision)
VALUES (9001, '2026-06-01', 'Paga', 1000.00, 'user@test.com', 9001);

-- ─────────────────────────────────────────────────
--  EVENTOS
--  RNE1: pais_sede admin ('Estados Unidos') debe
--  coincidir con pais_estadio (estadios 1,2,3 = EU)
-- ─────────────────────────────────────────────────
INSERT INTO evento (id_evento, fecha_evento, hora_evento, id_estadio, equipo_local, equipo_visitante, mail_admin) VALUES
(9001, '2026-06-16', '18:00:00', 1, 'México',  'Brasil',    'admin@demo.com'),  -- vencido
(9002, '2026-06-17', '23:59:00', 2, 'Uruguay', 'Argentina', 'admin@demo.com'),  -- hoy tarde
(9003, '2026-06-18', '20:00:00', 3, 'España',  'Francia',   'admin@demo.com');  -- mañana

-- ─────────────────────────────────────────────────
--  HABILITAR SECTORES Y ASIGNAR FUNCIONARIO
-- ─────────────────────────────────────────────────
INSERT INTO habilita  (id_evento, id_estadio, nombre_sector) VALUES
(9001, 1, 'A'), (9002, 2, 'B'), (9003, 3, 'C');

INSERT INTO asignacion (id_evento, id_estadio, nombre_sector, mail_funcionario) VALUES
(9001, 1, 'A', 'funcionario@demo.com'),
(9002, 2, 'B', 'funcionario@demo.com'),
(9003, 3, 'C', 'funcionario@demo.com');

-- ─────────────────────────────────────────────────
--  ENTRADAS
--  costo_entrada debe coincidir con costo_sector:
--  sector A=550, B=300, C=150  (trigger RNE39)
--  mail_titular será reemplazado por el trigger
--  trg_comprador_titular_entradas con mail_comprador
-- ─────────────────────────────────────────────────
INSERT INTO entrada (id_entrada, costo_entrada, estado_entrada, id_compra, id_evento, id_estadio, nombre_sector, mail_titular) VALUES
(9001, 550.00, 'Emitida', 9001, 9001, 1, 'A', 'user@test.com'),
(9002, 300.00, 'Emitida', 9001, 9002, 2, 'B', 'user@test.com'),
(9003, 150.00, 'Emitida', 9001, 9003, 3, 'C', 'user@test.com');

-- ─────────────────────────────────────────────────
--  TOKENS INICIALES
--  CRÍTICO: usar @ahora para que los dos timestamps
--  sean idénticos y el CHECK de la BD se cumpla:
--  fecha_hora_expiracion = fecha_hora_vigencia + 30s
-- ─────────────────────────────────────────────────
SET @ahora = NOW();

INSERT INTO token (codigo_qr, estado_token, fecha_hora_vigencia, fecha_hora_expiracion, id_entrada) VALUES
('token-evento-vencido-001',      'Activo', @ahora, DATE_ADD(@ahora, INTERVAL 30 SECOND), 9001),
('token-evento-casi-vencido-001', 'Activo', @ahora, DATE_ADD(@ahora, INTERVAL 30 SECOND), 9002),
('token-evento-futuro-001',       'Activo', @ahora, DATE_ADD(@ahora, INTERVAL 30 SECOND), 9003);

-- ─────────────────────────────────────────────────
--  VERIFICACIÓN INICIAL
-- ─────────────────────────────────────────────────
SELECT
    t.codigo_qr,
    t.estado_token,
    t.fecha_hora_vigencia,
    t.fecha_hora_expiracion,
    TIMESTAMPDIFF(SECOND, NOW(), t.fecha_hora_expiracion) AS expira_en_seg,
    e.id_entrada,
    e.estado_entrada,
    CONCAT(eq1.nombre_equipo, ' vs ', eq2.nombre_equipo)  AS partido,
    CASE
        WHEN CONCAT(ev.fecha_evento,' ',ev.hora_evento) < NOW() THEN '❌ VENCIDO'
        ELSE '✅ FUTURO'
    END AS estado_evento
FROM token t
INNER JOIN entrada e   ON t.id_entrada        = e.id_entrada
INNER JOIN evento  ev  ON e.id_evento          = ev.id_evento
INNER JOIN equipo  eq1 ON ev.equipo_local      = eq1.nombre_equipo
INNER JOIN equipo  eq2 ON ev.equipo_visitante  = eq2.nombre_equipo
ORDER BY ev.fecha_evento, ev.hora_evento;