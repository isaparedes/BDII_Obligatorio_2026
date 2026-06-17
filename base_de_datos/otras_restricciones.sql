-- OTRAS RESTRICCIONES NO ESTRUCTURALES (RNE)
USE CD_Grupo6;

-- RESTRICCIONES ESTABLECIDAS EN EL MODELO CONCEPTUAL:

-- RNE1: un administrador solamente puede gestionar estadios y eventos dentro de su jurisdicción geográfica.
DELIMITER //
CREATE TRIGGER trg_admin_solo_alta_jurisdiccion
BEFORE INSERT ON evento
FOR EACH ROW
BEGIN
    IF (
        SELECT a.pais_sede
        FROM administrador a
        WHERE a.mail = NEW.mail_admin
    ) <> (
        SELECT e.pais_estadio
        FROM estadio e
        WHERE e.id_estadio = NEW.id_estadio
    )
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El administrador solo puede gestionar eventos dentro de su jurisdicción geográfica';
    END IF;
END //
DELIMITER ;

-- RNE3: fecha_asignacion de un administrador debe ser menor o igual a la fecha_evento de un evento al que dicho administrador dio de alta.
DELIMITER //
CREATE TRIGGER trg_asginacion_previa_evento
BEFORE INSERT ON evento
FOR EACH ROW
BEGIN
    IF (
        SELECT fecha_asignacion
        FROM administrador
        WHERE mail = NEW.mail_admin
    ) > NEW.fecha_evento
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La fecha de asignación del administrador debe ser anterior al evento a dar de alta';
    END IF;
END //
DELIMITER ;

-- RNE4: la fecha_registro de un usuario_general debe ser menor o igual a la fecha_compra de una compra efectuada por dicho usuario_general.
DELIMITER //
CREATE TRIGGER trg_registro_previo_compra
BEFORE INSERT ON compra
FOR EACH ROW
BEGIN
    IF (
        SELECT fecha_registro
        FROM usuario_general
        WHERE mail = NEW.mail_comprador
    ) > NEW.fecha_compra
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La fecha de registro del usuario debe ser anterior a la compra a realizar';
    END IF;
END //
DELIMITER ;

-- RNE5: la fecha_registro de un usuario_general debe ser menor o igual a la fecha_transferencia de una transferencia llevada a cabo por dicho usuario_general.
DELIMITER //
CREATE TRIGGER trg_registro_previo_transferencia
BEFORE INSERT ON transferencia
FOR EACH ROW
BEGIN
    IF (
        SELECT fecha_registro
        FROM usuario_general
        WHERE mail = NEW.mail_remitente
    ) > NEW.fecha_transferencia
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La fecha de registro del remitente debe ser anterior a la transferencia a llevar a cabo';
    END IF;
END //
DELIMITER ;

-- RNE6: la fecha_registro de un usuario_general debe ser menor o igual a la fecha_aceptacion de una transferencia en la que dicho usuario_general fue el receptor de una entrada.
DELIMITER //
CREATE TRIGGER trg_registro_previo_aceptacion
BEFORE UPDATE ON transferencia
FOR EACH ROW
BEGIN
    IF NEW.estado_transferencia = 'Aceptada'
    AND (
        SELECT fecha_registro
        FROM usuario_general
        WHERE mail = NEW.mail_destinatario
    ) > NEW.fecha_aceptacion
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La fecha de registro del destinatario debe ser anterior a la fecha de aceptación de una transferencia';
    END IF;
END //
DELIMITER ;

-- RNE11: la cantidad de entradas vendidas para un sector durante un evento no puede superar la capacidad de dicho sector.
DELIMITER //
CREATE TRIGGER trg_capacidad_max_sector
BEFORE INSERT ON entrada
FOR EACH ROW
BEGIN
    IF (
        SELECT COUNT(*)
        FROM entrada
        WHERE id_evento = NEW.id_evento
        AND id_estadio = NEW.id_estadio
        AND nombre_sector = NEW.nombre_sector
    ) >= (
        SELECT capacidad
        FROM sector
        WHERE id_estadio = NEW.id_estadio
        AND nombre_sector = NEW.nombre_sector
    )
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El sector ha alcanzado su capacidad máxima';
    END IF;
END // 
DELIMITER ;

-- RNE14: no pueden ocurrir simultáneamente dos eventos en un mismo estadio.
DELIMITER //
CREATE TRIGGER trg_eventos_no_simultaneos
BEFORE INSERT ON evento
FOR EACH ROW
BEGIN
    IF EXISTS (
        SELECT 1
        FROM evento
        WHERE id_estadio = NEW.id_estadio
        AND fecha_evento = NEW.fecha_evento
        AND hora_evento = NEW.hora_evento
    )
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Ya existe un evento en ese estadio para esa fecha';
    END IF;
END //
DELIMITER ;

-- RNE15: un usuario_general solamente puede realizar la compra de entradas si su estado_verificación tiene el valor 'Aprobado'.
DELIMITER //
CREATE TRIGGER trg_usuario_verificado_compra
BEFORE INSERT ON compra
FOR EACH ROW
BEGIN
    IF (
        SELECT estado_verificacion
        FROM usuario_general
        WHERE mail = NEW.mail_comprador
    ) <> 'Aprobado'
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El usuario debe tener verificación aprobada para comprar';
    END IF;
END //
DELIMITER ;

-- RNE16: al efectuarse una compra automáticamente queda registrado como titular de cada entrada adquirida el usuario_general que realizó dicha compra.
DELIMITER //
CREATE TRIGGER trg_comprador_titular_entradas
BEFORE INSERT ON entrada
FOR EACH ROW
BEGIN
    SET NEW.mail_titular = (
        SELECT mail_comprador
        FROM compra
        WHERE id_compra = NEW.id_compra
    );
END //
DELIMITER ;

-- RNE19: el monto_total de la compra se calcula a partir de la suma del costo_entrada de cada entrada adquirida en dicha transacción.
DELIMITER //
CREATE TRIGGER trg_calculo_monto_total
AFTER INSERT ON entrada
FOR EACH ROW
BEGIN
    UPDATE compra
    SET monto_total = (
        SELECT SUM(costo_entrada)
        FROM entrada
        WHERE id_compra = NEW.id_compra
    )
    WHERE id_compra = NEW.id_compra;
END //
DELIMITER ;

-- RNE25: un usuario_general no puede estar asociado a más de 5 instancias de entrada adquiridas a través de una misma compra.
DELIMITER //
CREATE TRIGGER trg_max_5_entradas_por_compra
BEFORE INSERT ON entrada
FOR EACH ROW
BEGIN
    IF (
        SELECT COUNT(*)
        FROM entrada
        WHERE id_compra = NEW.id_compra
    ) >= 5
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Una compra no puede tener más de 5 entradas asociadas';
    END IF;
END //
DELIMITER ;

-- RNE28: una entrada no puede estar vinculada a más de 3 registros de transferencia.
DELIMITER //
CREATE TRIGGER trg_max_3_transferencias_por_entrada
BEFORE INSERT ON transferencia
FOR EACH ROW
BEGIN
    IF (
        SELECT COUNT(*)
        FROM transferencia
        WHERE id_entrada = NEW.id_entrada
    ) >= 3
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Una entrada no puede transferirse más de 3 veces';
    END IF;
END //
DELIMITER ;

-- RNE30: el cambio de titular de entrada se hace efectivo solo cuando estado_transferencia toma el valor 'Aceptada'.
DELIMITER //
CREATE TRIGGER trg_cambio_titular
AFTER UPDATE ON transferencia
FOR EACH ROW
BEGIN
    IF NEW.estado_transferencia = 'Aceptada'
    AND OLD.estado_transferencia <> 'Aceptada'
    THEN
        UPDATE entrada
        SET mail_titular = NEW.mail_destinatario
        WHERE id_entrada = NEW.id_entrada;
    END IF;
END //
DELIMITER ;

-- RNE31: una entrada solo puede ser transferida si la fecha_evento asociada es posterior al momento en que se inicia la transferencia (fecha_transferencia).
DELIMITER //
CREATE TRIGGER trg_transferencia_evento_pasado
BEFORE INSERT ON transferencia
FOR EACH ROW
BEGIN
    IF (
        SELECT fecha_evento
        FROM evento
        INNER JOIN entrada ON evento.id_evento = entrada.id_evento
        WHERE entrada.id_entrada = NEW.id_entrada
    ) <= CURDATE()
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'No se puede transferir una entrada de un evento ya ocurrido';
    END IF;
END //
DELIMITER ;

-- RNE39: el costo_entrada de una entrada debe ser igual al costo_sector del sector al que dicha entrada corresponde en el momento de su emisión.
DELIMITER //
CREATE TRIGGER trg_costo_entrada_costo_sector
BEFORE INSERT ON entrada
FOR EACH ROW
BEGIN
    IF NEW.costo_entrada <> (
        SELECT costo_sector
        FROM sector
        WHERE id_estadio = NEW.id_estadio
        AND nombre_sector = NEW.nombre_sector
    )
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El costo de la entrada debe coincidir con el costo del sector';
    END IF;
END //
DELIMITER ;

-- RNE41: una vez que el token de una entrada formó parte de una validación por un dispositivo la entrada no se puede transferir ni volver a utilizar.
DELIMITER //
CREATE TRIGGER trg_no_transferir_entrada_consumida
BEFORE INSERT ON transferencia
FOR EACH ROW
BEGIN
    IF (
        SELECT estado_entrada
        FROM entrada
        WHERE id_entrada = NEW.id_entrada
    ) = 'Consumida'
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'No se puede transferir una entrada ya consumida';
    END IF;
END //
DELIMITER ;
DELIMITER //
-- El estado_entrada de una entrada queda automáticamente registrado como 'Consumida'.
CREATE TRIGGER trg_consumir_entrada
AFTER UPDATE ON token
FOR EACH ROW
BEGIN
    IF NEW.id_dispositivo_valida IS NOT NULL 
    AND OLD.id_dispositivo_valida IS NULL
    THEN
        UPDATE entrada
        SET estado_entrada = 'Consumida'
        WHERE id_entrada = NEW.id_entrada;
    END IF;
END //
DELIMITER ;

-- RNE42: el token utilizado en una validación debe ser un token con estado_token 'Activo' de una entrada al momento del escaneo; 
-- un token con una fecha expirada (estado_token: 'Expirado') al momento del escaneo es inválido y debe ser rechazado. 
DELIMITER //
CREATE TRIGGER trg_token_activo_para_validar
BEFORE UPDATE ON token
FOR EACH ROW
BEGIN
    IF NEW.id_dispositivo_valida IS NOT NULL
    AND OLD.id_dispositivo_valida IS NULL
    AND OLD.estado_token <> 'Activo'
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Solo se puede validar un token activo';
    END IF;
END //
DELIMITER ;

-- RESTRICCIONES ESTABLECIDAS EN EL MODELO LÓGICO:

-- RNE: π id_compra(compra) - π id_compra(entrada) ≠ ∅ → ERROR
DELIMITER //
CREATE TRIGGER trg_compra_min_una_entrada
BEFORE DELETE ON entrada
FOR EACH ROW
BEGIN
    IF (
        SELECT COUNT(*)
        FROM entrada
        WHERE id_compra = OLD.id_compra
    ) = 1
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Una compra debe tener al menos una entrada asociada';
    END IF;
END //
DELIMITER ;

-- RNE: π id_evento(evento) - π id_evento(habilita) ≠ ∅ → ERROR
DELIMITER //
CREATE TRIGGER trg_evento_min_un_sector
BEFORE DELETE ON habilita
FOR EACH ROW
BEGIN
    IF (
        SELECT COUNT(*)
        FROM habilita
        WHERE id_evento = OLD.id_evento
    ) = 1
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Un evento debe tener al menos un sector habilitado';
    END IF;
END //
DELIMITER ;
 
-- RNE: π id_estadio(estadio) - π id_estadio(sector) ≠ ∅ → ERROR
DELIMITER //
CREATE TRIGGER trg_estadio_min_un_sector
BEFORE DELETE ON sector
FOR EACH ROW
BEGIN
    IF (
        SELECT COUNT(*)
        FROM sector
        WHERE id_estadio = OLD.id_estadio
    ) = 1
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Un estadio debe tener al menos un sector';
    END IF;
END //
DELIMITER ;

-- RNE: π id_evento, id_estadio, nombre_sector(habilita) - π id_evento, id_estadio, nombre_sector(asignacion) ≠ ∅ → ERROR
DELIMITER //
CREATE TRIGGER trg_sector_min_un_funcionario
BEFORE DELETE ON asignacion
FOR EACH ROW
BEGIN
    IF (
        SELECT COUNT(*)
        FROM asignacion
        WHERE id_evento = OLD.id_evento
        AND id_estadio = OLD.id_estadio
        AND nombre_sector = OLD.nombre_sector
    ) = 1
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Un sector habilitado debe tener al menos un funcionario asignado';
    END IF;
END //
DELIMITER ;

-- RNE: π id_entrada(entrada) - π id_entrada(token) ≠ ∅ → ERROR
DELIMITER //
CREATE TRIGGER trg_entrada_min_un_token
BEFORE DELETE ON token
FOR EACH ROW
BEGIN
    IF (
        SELECT COUNT(*)
        FROM token
        WHERE id_entrada = OLD.id_entrada
    ) = 1
    THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Una entrada debe tener al menos un token asociado';
    END IF;
END //
DELIMITER ;
