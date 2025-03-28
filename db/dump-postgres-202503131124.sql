PGDMP  2                    }            postgres    17.4    17.0     �           0    0    ENCODING    ENCODING        SET client_encoding = 'UTF8';
                           false            �           0    0 
   STDSTRINGS 
   STDSTRINGS     (   SET standard_conforming_strings = 'on';
                           false            �           0    0 
   SEARCHPATH 
   SEARCHPATH     8   SELECT pg_catalog.set_config('search_path', '', false);
                           false            �           1262    5    postgres    DATABASE     n   CREATE DATABASE postgres WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en-GB';
    DROP DATABASE postgres;
                     postgres    false            �           0    0    DATABASE postgres    COMMENT     N   COMMENT ON DATABASE postgres IS 'default administrative connection database';
                        postgres    false    4854                        2615    2200    public    SCHEMA        CREATE SCHEMA public;
    DROP SCHEMA public;
                     pg_database_owner    false            �           0    0    SCHEMA public    COMMENT     6   COMMENT ON SCHEMA public IS 'standard public schema';
                        pg_database_owner    false    4            �            1259    16391    EmployeeAttendance    TABLE     �   CREATE TABLE public."EmployeeAttendance" (
    id uuid NOT NULL,
    employee_id character varying NOT NULL,
    tapped_at timestamp without time zone
);
 (   DROP TABLE public."EmployeeAttendance";
       public         heap r       postgres    false    4            �            1259    16384    MsEmployees    TABLE     !  CREATE TABLE public."MsEmployees" (
    id character varying NOT NULL,
    name character varying NOT NULL,
    birth_date date NOT NULL,
    gender character varying NOT NULL,
    address character varying NOT NULL,
    contact_number character varying NOT NULL,
    is_active boolean
);
 !   DROP TABLE public."MsEmployees";
       public         heap r       postgres    false    4            �          0    16391    EmployeeAttendance 
   TABLE DATA           J   COPY public."EmployeeAttendance" (id, employee_id, tapped_at) FROM stdin;
    public               postgres    false    218   Z       �          0    16384    MsEmployees 
   TABLE DATA           i   COPY public."MsEmployees" (id, name, birth_date, gender, address, contact_number, is_active) FROM stdin;
    public               postgres    false    217   �       ]           2606    16397 (   EmployeeAttendance employeeattendance_pk 
   CONSTRAINT     h   ALTER TABLE ONLY public."EmployeeAttendance"
    ADD CONSTRAINT employeeattendance_pk PRIMARY KEY (id);
 T   ALTER TABLE ONLY public."EmployeeAttendance" DROP CONSTRAINT employeeattendance_pk;
       public                 postgres    false    218            [           2606    16390    MsEmployees msemployee_pk 
   CONSTRAINT     Y   ALTER TABLE ONLY public."MsEmployees"
    ADD CONSTRAINT msemployee_pk PRIMARY KEY (id);
 E   ALTER TABLE ONLY public."MsEmployees" DROP CONSTRAINT msemployee_pk;
       public                 postgres    false    217            �   h   x�m�K�0�s���'���l,��?�J�`���%�v�
6�9)�'����y5��"D]�?f�#�j�}p�)+�J倌P�m<`���um7=���.�� Yo�      �   �  x�m��N1@����p53��"\$�����x�(�,N���~}ǄTx�=>�3��#�[���ׇ�ƹ;Yo��<�! ���\K�17��X�b %"F�Ȍ���Yʣ����b~%�A�en$$�	�)�b6����ܝOU��66Z���^�N���^q�!)]�ޜ����_�8��������%J�%=&��l=w�r�ޫ����v~�j#O1*�� �����q�ޤ����a�@9P��DoV�����4�VJa���~��10)�bj�B��̅���W��6v�Y=�
�̠jb�4�r3(�*����`uEG�T�z�ۗ���t�j��I�e��fyh#��E��3���vaRm2���ݥ�Ӧo�����;1��_n��,�?hb�k     