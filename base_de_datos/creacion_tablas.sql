-- CREACIÓN DE TABLAS EN BASE DE DATOS DEL PROYECTO: CD_Grupo6
USE CD_Grupo6;

-- TABLA: usuario
CREATE TABLE usuario (
    mail VARCHAR(255) PRIMARY KEY,
    contrasena VARCHAR(255) NOT NULL,
    pais_documento VARCHAR(50) NOT NULL,
    tipo_documento VARCHAR(30) NOT NULL,
    numero_documento VARCHAR(30) NOT NULL,
    pais_direccion VARCHAR(50) NOT NULL,
    localidad VARCHAR(50) NOT NULL,
    calle VARCHAR(50) NOT NULL,
    numero_calle INT NOT NULL,
    codigo_postal VARCHAR(20) NOT NULL,
    UNIQUE (pais_documento, tipo_documento, numero_documento)
);

-- TABLA: usuario_telefono
CREATE TABLE usuario_telefono (
    mail_usuario VARCHAR(255),
    numero_telefono VARCHAR(20),
    PRIMARY KEY (mail_usuario, numero_telefono),
    FOREIGN KEY (mail_usuario) REFERENCES usuario(mail) ON DELETE CASCADE
);

-- TABLA: funcionario
CREATE TABLE funcionario (
    mail VARCHAR(255) PRIMARY KEY,
    numero_legajo INT NOT NULL UNIQUE,
    FOREIGN KEY (mail) REFERENCES usuario(mail) ON DELETE CASCADE
);

-- TABLA: administrador
CREATE TABLE administrador (
    mail VARCHAR(255) PRIMARY KEY,
    fecha_asignacion DATE NOT NULL,
    pais_sede VARCHAR(50) NOT NULL,
    FOREIGN KEY (mail) REFERENCES usuario(mail) ON DELETE CASCADE
);

-- TABLA: usuario_general
CREATE TABLE usuario_general (
    mail VARCHAR(255) PRIMARY KEY,
    estado_verificacion VARCHAR(20) NOT NULL DEFAULT 'Pendiente',
    fecha_registro DATE NOT NULL,
    FOREIGN KEY (mail) REFERENCES usuario(mail) ON DELETE CASCADE,
    CHECK (estado_verificacion IN ('Pendiente', 'Aprobado', 'No aprobado')) -- RNE7
);

-- TABLA: comision
CREATE TABLE comision (
    id_comision INT AUTO_INCREMENT PRIMARY KEY,
    valor_comision DECIMAL(5,2) NOT NULL,
    fecha_inicio DATE NOT NULL,
    fecha_fin DATE NOT NULL,
    CHECK (valor_comision > 0), -- RNE22
    CHECK (fecha_inicio < fecha_fin) -- RNE24
);

-- TABLA: compra
CREATE TABLE compra (
    id_compra INT AUTO_INCREMENT PRIMARY KEY,
    fecha_compra DATE NOT NULL,
    estado_compra VARCHAR(20) NOT NULL DEFAULT 'Pendiente',
    monto_total DECIMAL(10,2) NOT NULL DEFAULT 1,
    mail_comprador VARCHAR(255) NOT NULL,
    id_comision INT NOT NULL,
    FOREIGN KEY (mail_comprador) REFERENCES usuario_general(mail),
    FOREIGN KEY (id_comision) REFERENCES comision(id_comision),
    CHECK (estado_compra IN ('Pendiente', 'Confirmada', 'Paga')), -- RNE18
    CHECK (monto_total > 0) -- RNE20
);

-- TABLA: estadio
CREATE TABLE estadio (
    id_estadio INT AUTO_INCREMENT PRIMARY KEY,
    nombre_estadio VARCHAR(50) NOT NULL,
    pais_estadio VARCHAR(50) NOT NULL,
    ciudad_estadio VARCHAR(50) NOT NULL,
    calle_estadio VARCHAR(50) NOT NULL,
    numero_estadio INT NOT NULL
);

-- TABLA: sector
CREATE TABLE sector (
    id_estadio INT,
    nombre_sector CHAR(1),
    costo_sector DECIMAL(10,2) NOT NULL,
    capacidad INT NOT NULL,
    PRIMARY KEY (id_estadio, nombre_sector),
    FOREIGN KEY (id_estadio) REFERENCES estadio(id_estadio) ON DELETE CASCADE,
    CHECK (nombre_sector IN ('A', 'B', 'C', 'D')), -- RNE8
    CHECK (costo_sector > 0), -- RNE10
    CHECK (capacidad > 0) -- RNE9
);

-- TABLA: equipo
CREATE TABLE equipo (
    nombre_equipo VARCHAR(50) PRIMARY KEY
);

-- TABLA: evento
CREATE TABLE evento (
    id_evento INT AUTO_INCREMENT PRIMARY KEY,
    fecha_evento DATE NOT NULL,
    hora_evento TIME NOT NULL,
    id_estadio INT NOT NULL,
    equipo_local VARCHAR(50) NOT NULL,
    equipo_visitante VARCHAR(50) NOT NULL,
    mail_admin VARCHAR(255) NOT NULL,
    FOREIGN KEY (id_estadio) REFERENCES estadio(id_estadio),
    FOREIGN KEY (equipo_local) REFERENCES equipo(nombre_equipo),
    FOREIGN KEY (equipo_visitante) REFERENCES equipo(nombre_equipo),
    FOREIGN KEY (mail_admin) REFERENCES administrador(mail),
    CHECK (equipo_local <> equipo_visitante) -- RNE13
);

-- TABLA: habilita
CREATE TABLE habilita (
    id_evento INT,
    id_estadio INT,
    nombre_sector CHAR(1),
    PRIMARY KEY (id_evento, id_estadio, nombre_sector),
    FOREIGN KEY (id_evento) REFERENCES evento(id_evento) ON DELETE CASCADE,
    FOREIGN KEY (id_estadio, nombre_sector) REFERENCES sector(id_estadio, nombre_sector) ON DELETE CASCADE 
);

-- TABLA: asignacion
CREATE TABLE asignacion (
    id_evento INT,
    id_estadio INT,
    nombre_sector CHAR(1),
    mail_funcionario VARCHAR(255),
    PRIMARY KEY (id_evento, id_estadio, nombre_sector, mail_funcionario),
    FOREIGN KEY (id_evento) REFERENCES evento(id_evento) ON DELETE CASCADE,
    FOREIGN KEY (id_estadio, nombre_sector) REFERENCES sector(id_estadio, nombre_sector) ON DELETE CASCADE,
    FOREIGN KEY (mail_funcionario) REFERENCES funcionario(mail) ON DELETE CASCADE 
);

-- TABLA: entrada
CREATE TABLE entrada (
    id_entrada INT AUTO_INCREMENT PRIMARY KEY,
    costo_entrada DECIMAL(10,2) NOT NULL,
    estado_entrada VARCHAR(20) NOT NULL DEFAULT 'Emitida',
    id_compra INT NOT NULL,
    id_evento INT NOT NULL,
    id_estadio INT NOT NULL,
    nombre_sector CHAR(1) NOT NULL,
    mail_titular VARCHAR(255) NOT NULL,
    FOREIGN KEY (id_compra) REFERENCES compra(id_compra),
    FOREIGN KEY (id_evento, id_estadio, nombre_sector) REFERENCES habilita(id_evento, id_estadio, nombre_sector),
    FOREIGN KEY (mail_titular) REFERENCES usuario_general(mail),
    CHECK (costo_entrada > 0), -- RNE38
    CHECK (estado_entrada IN ('Emitida', 'Consumida')) -- RNE40
);

-- TABLA: transferencia
CREATE TABLE transferencia (
    id_transferencia INT AUTO_INCREMENT PRIMARY KEY,
    fecha_transferencia DATE NOT NULL,
    fecha_aceptacion DATE,
    estado_transferencia VARCHAR(20) NOT NULL DEFAULT 'En proceso',
    id_entrada INT NOT NULL,
    mail_remitente VARCHAR(255) NOT NULL,
    mail_destinatario VARCHAR(255) NOT NULL,
    FOREIGN KEY (id_entrada) REFERENCES entrada(id_entrada),
    FOREIGN KEY (mail_remitente) REFERENCES usuario_general(mail),
    FOREIGN KEY (mail_destinatario) REFERENCES usuario_general(mail),
    CHECK (fecha_transferencia < fecha_aceptacion), -- RNE27
    CHECK (estado_transferencia IN ('En proceso', 'Aceptada', 'Rechazada')), -- RNE29
    CHECK (mail_remitente <> mail_destinatario) -- RNE32
);

-- TABLA: dispositivo
CREATE TABLE dispositivo (
    id_dispositivo INT AUTO_INCREMENT PRIMARY KEY,
    mail_funcionario VARCHAR(255) NOT NULL,
    FOREIGN KEY (mail_funcionario) REFERENCES funcionario(mail)
);

-- TABLA: token
CREATE TABLE token (
    codigo_qr VARCHAR(255) PRIMARY KEY,
    estado_token VARCHAR(20) NOT NULL DEFAULT 'Activo',
    fecha_hora_vigencia DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    fecha_hora_expiracion DATETIME,
    fecha_hora_validacion DATETIME,
    id_entrada INT NOT NULL,
    id_dispositivo_valida INT,
    FOREIGN KEY (id_entrada) REFERENCES entrada(id_entrada),
    FOREIGN KEY (id_dispositivo_valida) REFERENCES dispositivo(id_dispositivo),
    CHECK (estado_token IN ('Activo', 'Expirado')), -- RNE36
    CHECK (fecha_hora_vigencia < fecha_hora_validacion),
    CHECK (fecha_hora_expiracion = DATE_ADD(fecha_hora_vigencia, INTERVAL 30 SECOND)) -- RNE35
);





