
USE CD_Grupo6;

-- VISTA 1: vw_ocupacion_sectores
-- Controlar en tiempo real la venta, disponibilidad y el porcentaje de ocupación de cada sector por evento (Herramienta para auditar la RNE11).
CREATE OR REPLACE VIEW vw_ocupacion_sectores AS
SELECT 
    e.id_evento,
    e.fecha_evento,
    e.hora_evento,
    eq_l.nombre_equipo AS equipo_local,
    eq_v.nombre_equipo AS equipo_visitante,
    es.nombre_estadio,
    s.nombre_sector,
    s.costo_sector,
    s.capacidad AS capacidad_total,
    COUNT(en.id_entrada) AS entradas_vendidas,
    (s.capacidad - COUNT(en.id_entrada)) AS disponibilidad_restante,
    ROUND((COUNT(en.id_entrada) / s.capacidad) * 100, 2) AS porcentaje_ocupacion
FROM evento e
JOIN estadio es ON e.id_estadio = es.id_estadio
JOIN habilita h ON e.id_evento = h.id_evento
JOIN sector s ON h.id_estadio = s.id_estadio AND h.nombre_sector = s.nombre_sector
LEFT JOIN entrada en ON e.id_evento = en.id_evento 
                    AND h.id_estadio = en.id_estadio 
                    AND h.nombre_sector = en.nombre_sector
JOIN equipo eq_l ON e.equipo_local = eq_l.nombre_equipo
JOIN equipo eq_v ON e.equipo_visitante = eq_v.nombre_equipo
GROUP BY e.id_evento, h.id_estadio, h.nombre_sector;


-- VISTA 2: vw_mis_entradas
--  Mostrar a los clientes finales (usuario_general) sus entradas vigentes, detallando el partido, el estadio, el sector asignado y el QR activo.
CREATE OR REPLACE VIEW vw_mis_entradas AS
SELECT 
    en.id_entrada,
    en.mail_titular,
    en.estado_entrada,
    en.costo_entrada,
    e.id_evento,
    e.fecha_evento,
    e.hora_evento,
    e.equipo_local,
    e.equipo_visitante,
    es.nombre_estadio,
    es.ciudad_estadio,
    en.nombre_sector,
    t.codigo_qr,
    t.estado_token
FROM entrada en
JOIN evento e ON en.id_evento = e.id_evento
JOIN estadio es ON en.id_estadio = es.id_estadio
LEFT JOIN token t ON en.id_entrada = t.id_entrada AND t.estado_token = 'Activo';


-- VISTA 3: vw_validacion_tokens
-- Objetivo: Proporcionar a la app del Funcionario/Dispositivo los datos necesarios para validar la identidad del portador y la vigencia del QR en los accesos.
CREATE OR REPLACE VIEW vw_validacion_tokens AS
SELECT 
    t.codigo_qr,
    t.estado_token,
    t.fecha_hora_expiracion,
    en.id_entrada,
    en.estado_entrada,
    en.nombre_sector,
    u.pais_documento,
    u.tipo_documento,
    u.numero_documento,
    CONCAT(e.equipo_local, ' vs ', e.equipo_visitante) AS partido,
    es.nombre_estadio
FROM token t
JOIN entrada en ON t.id_entrada = en.id_entrada
JOIN usuario u ON en.mail_titular = u.mail
JOIN evento e ON en.id_evento = e.id_evento
JOIN estadio es ON en.id_estadio = es.id_estadio;


-- VISTA 4: vw_auditoria_compras
-- Permitir al área financiera auditar las transacciones, verificando montos, cantidad de entradas adquiridas y la comisión calculada (RNE43).
CREATE OR REPLACE VIEW vw_auditoria_compras AS
SELECT 
    c.id_compra,
    c.fecha_compra,
    c.estado_compra,
    c.mail_comprador,
    u.pais_documento,
    u.numero_documento,
    c.monto_total,
    co.valor_comision AS porcentaje_comision,
    ROUND(c.monto_total * (co.valor_comision / 100), 2) AS ganancia_comision,
    COUNT(en.id_entrada) AS cantidad_entradas
FROM compra c
JOIN comision co ON c.id_comision = co.id_comision
JOIN usuario_general ug ON c.mail_comprador = ug.mail
JOIN usuario u ON ug.mail = u.mail
LEFT JOIN entrada en ON c.id_compra = en.id_compra
GROUP BY c.id_compra;


-- VISTA 5: vw_rastreo_transferencias
-- Seguir la trazabilidad histórica de las entradas cuando cambian de dueño, facilitando el control de las restricciones RNE30, RNE31 y RNE32.

CREATE OR REPLACE VIEW vw_rastreo_transferencias AS
SELECT 
    t.id_transferencia,
    t.id_entrada,
    t.fecha_transferencia,
    t.fecha_aceptacion,
    t.estado_transferencia,
    t.mail_remitente AS de_usuario,
    t.mail_destinatario AS para_usuario,
    ev.id_evento,
    CONCAT(ev.equipo_local, ' vs ', ev.equipo_visitante) AS partido,
    ev.fecha_evento
FROM transferencia t
JOIN entrada en ON t.id_entrada = en.id_entrada
JOIN evento ev ON en.id_evento = ev.id_evento;