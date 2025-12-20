--
-- PostgreSQL database dump
--

\restrict 0rJNHsoFr8LJ7g3NC3pwFn3gnpDNFDqzAJx4eBUK1wvICewPqjs1Wh7FXRvk0iT

-- Dumped from database version 18.0
-- Dumped by pg_dump version 18.0

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

DROP DATABASE boyamakinedevami;
--
-- Name: boyamakinedevami; Type: DATABASE; Schema: -; Owner: -
--

CREATE DATABASE boyamakinedevami WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'English_United States.1254';


\unrestrict 0rJNHsoFr8LJ7g3NC3pwFn3gnpDNFDqzAJx4eBUK1wvICewPqjs1Wh7FXRvk0iT
\connect boyamakinedevami
\restrict 0rJNHsoFr8LJ7g3NC3pwFn3gnpDNFDqzAJx4eBUK1wvICewPqjs1Wh7FXRvk0iT

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: bakim_uyarilari(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.bakim_uyarilari() RETURNS TABLE(parca_adi character varying, son_bakim_tarihi date, gecen_gun integer, bakim_periyodu character varying, durum character varying, uyari_mesaji text)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    WITH son_bakimlar AS (
        -- Her parça için son bakım tarihini bul
        SELECT 
            bakimturu,
            MAX(bakimtarihi) as son_tarih
        FROM bakimkaydi
        WHERE bakimturu IN ('Karıştırıcı Motor', 'Boya Pompası', 'Filtre Sistemi')
        GROUP BY bakimturu
    ),
    parca_listesi AS (
        -- 3 parça tanımı
        SELECT 'Karıştırıcı Motor'::VARCHAR as parca, 1 as gun_limit
        UNION ALL
        SELECT 'Boya Pompası'::VARCHAR, 7
        UNION ALL
        SELECT 'Filtre Sistemi'::VARCHAR, 30
    )
    SELECT 
        pl.parca as parca_adi,
        sb.son_tarih as son_bakim_tarihi,
        COALESCE(CURRENT_DATE - sb.son_tarih, 999) as gecen_gun,
        CASE 
            WHEN pl.gun_limit = 1 THEN 'Günlük'
            WHEN pl.gun_limit = 7 THEN 'Haftalık'
            WHEN pl.gun_limit = 30 THEN 'Aylık'
        END::VARCHAR as bakim_periyodu,
        CASE 
            WHEN sb.son_tarih IS NULL THEN 'YAPILMADI'
            WHEN (CURRENT_DATE - sb.son_tarih) > pl.gun_limit * 2 THEN 'KRİTİK'
            WHEN (CURRENT_DATE - sb.son_tarih) > pl.gun_limit THEN 'GECİKMİŞ'
            ELSE 'NORMAL'
        END::VARCHAR as durum,
        CASE 
            WHEN sb.son_tarih IS NULL THEN 
                '⚠️ ' || pl.parca || ' - HİÇ BAKIM YAPILMAMIŞ!'
            WHEN (CURRENT_DATE - sb.son_tarih) > pl.gun_limit * 2 THEN 
                '❗ ' || pl.parca || ' - KRİTİK! ' || (CURRENT_DATE - sb.son_tarih) || ' gün geçti!'
            WHEN (CURRENT_DATE - sb.son_tarih) > pl.gun_limit THEN 
                '❗ ' || pl.parca || ' - GECİKMİŞ! ' || (CURRENT_DATE - sb.son_tarih) || ' gün geçti.'
            ELSE 
                '✅ ' || pl.parca || ' - Normal (Son: ' || (CURRENT_DATE - sb.son_tarih) || ' gün önce)'
        END::TEXT as uyari_mesaji
    FROM parca_listesi pl
    LEFT JOIN son_bakimlar sb ON sb.bakimturu = pl.parca
    ORDER BY 
        CASE 
            WHEN sb.son_tarih IS NULL THEN 999
            ELSE CURRENT_DATE - sb.son_tarih
        END DESC;
END;
$$;


--
-- Name: bakim_yap(integer, character varying); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.bakim_yap(p_personel_rolno integer, p_parca_adi character varying) RETURNS TABLE(basarili boolean, mesaj text, bakim_no integer)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_bakimno INTEGER;
    v_personel_ad VARCHAR;
    v_personel_soyad VARCHAR;
    v_mevcut_kayit INTEGER;
BEGIN
    -- Personel kontrolü
    SELECT personelad || ' ' || personelsoyad INTO v_personel_ad
    FROM personel WHERE rolno = p_personel_rolno;
    
    IF v_personel_ad IS NULL THEN
        RETURN QUERY SELECT 
            FALSE, 
            '❌ HATA: Personel bulunamadı!'::TEXT, 
            NULL::INTEGER;
        RETURN;
    END IF;
    
    -- Parça adı kontrolü
    IF p_parca_adi NOT IN ('Karıştırıcı Motor', 'Boya Pompası', 'Filtre Sistemi') THEN
        RETURN QUERY SELECT 
            FALSE, 
            '❌ HATA: Geçersiz parça adı! (Karıştırıcı Motor, Boya Pompası veya Filtre Sistemi)'::TEXT,
            NULL::INTEGER;
        RETURN;
    END IF;
    
    -- Bu parça için daha önce bakım kaydı var mı kontrol et
    SELECT bakimno INTO v_mevcut_kayit
    FROM bakimkaydi 
    WHERE bakimturu = p_parca_adi
    ORDER BY bakimtarihi DESC
    LIMIT 1;
    
    IF v_mevcut_kayit IS NOT NULL THEN
        -- Mevcut kaydı güncelle
        UPDATE bakimkaydi 
        SET bakimtarihi = CURRENT_DATE,
            personelrolno = p_personel_rolno
        WHERE bakimno = v_mevcut_kayit
        RETURNING bakimno INTO v_bakimno;
        
        RETURN QUERY SELECT 
            TRUE, 
            ('✅ ' || p_parca_adi || ' bakımı tamamlandı! (Tarih güncellendi) Personel: ' || v_personel_ad)::TEXT,
            v_bakimno;
    ELSE
        -- İlk kez bakım yapılıyor, yeni kayıt ekle
        INSERT INTO bakimkaydi (bakimtarihi, bakimturu, personelrolno)
        VALUES (CURRENT_DATE, p_parca_adi, p_personel_rolno)
        RETURNING bakimno INTO v_bakimno;
        
        RETURN QUERY SELECT 
            TRUE, 
            ('✅ ' || p_parca_adi || ' bakımı tamamlandı! (İlk bakım) Personel: ' || v_personel_ad)::TEXT,
            v_bakimno;
    END IF;
END;
$$;


--
-- Name: boya_yap(character varying, character varying, character varying, character varying, character varying, character varying, integer, character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.boya_yap(p_musteri_ad character varying, p_musteri_soyad character varying, p_musteri_iletisim character varying, p_musteri_adres character varying, p_dukkan_ad character varying, p_dukkan_tel character varying, p_personel_rolno integer, p_renk_kodu character varying, p_baz_kg integer) RETURNS TABLE(basarili boolean, mesaj text, islem_no integer)
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_islemno INTEGER;
    v_personel_ad VARCHAR;
    v_personel_soyad VARCHAR;
    v_renk_ismi VARCHAR;
    v_toplam_pigment INTEGER;
    v_stok_mevcut INTEGER;
BEGIN
    -- Personel bilgisi al
    SELECT personelad, personelsoyad INTO v_personel_ad, v_personel_soyad
    FROM personel 
    WHERE rolno = p_personel_rolno;
    
    IF v_personel_ad IS NULL THEN
        RETURN QUERY SELECT FALSE, ('Personel bulunamadı!')::TEXT, NULL::INTEGER;
        RETURN;
    END IF;
    
    -- Renk bilgisi al
    SELECT renkismi INTO v_renk_ismi
    FROM hazirrenk 
    WHERE renkkodu = p_renk_kodu;
    
    IF v_renk_ismi IS NULL THEN
        RETURN QUERY SELECT FALSE, ('Renk bulunamadı!')::TEXT, NULL::INTEGER;
        RETURN;
    END IF;
    
    -- Pigment hesapla
    SELECT COALESCE(SUM(pigment_miktar_gr), 0) INTO v_toplam_pigment
    FROM renk_pigment_detay 
    WHERE renkkodu = p_renk_kodu;
    
    IF v_toplam_pigment = 0 THEN
        RETURN QUERY SELECT FALSE, 'Pigment form yok!'::TEXT, NULL::INTEGER;
        RETURN;
    END IF;
    
    -- Stok kontrol
    SELECT COALESCE(SUM(kalanpigmentgr), 0) INTO v_stok_mevcut
    FROM stok WHERE pigmentisim IS NOT NULL;
    
    IF v_stok_mevcut < v_toplam_pigment THEN
        RETURN QUERY SELECT FALSE, 
            (' Stok yetersiz! Mevcut: ' || v_stok_mevcut || 'gr, Gereken: ' || v_toplam_pigment || 'gr')::TEXT, 
            NULL::INTEGER;
        RETURN;
    END IF;
    
    -- ─░┼şlem no
    SELECT COALESCE(MAX(islemno), 0) + 1 INTO v_islemno FROM karisimkagidi;
    
    -- Insert karisimkagidi - DO─ŞRU SIRADA!
    INSERT INTO karisimkagidi (
        islemno, islemtarihi, 
        musteriad, musterisoyad, musteriiletisim, musteriadres,
        dukkanad, dukkantelno,
        personelad, personelsoyad,
        renkismi, renkkodu, bazkg
    ) VALUES (
        v_islemno, 
        CURRENT_TIMESTAMP,
        p_musteri_ad, 
        p_musteri_soyad, 
        p_musteri_iletisim, 
        p_musteri_adres,
        p_dukkan_ad, 
        p_dukkan_tel,
        v_personel_ad, 
        v_personel_soyad,
        v_renk_ismi, 
        p_renk_kodu, 
        p_baz_kg
    );
    
    -- Insert yapilanboya
    INSERT INTO yapilanboya (islemno, kullanilanbazkg, kullanilanpigmentgr)
    VALUES (v_islemno, p_baz_kg, v_toplam_pigment);
    
    RETURN QUERY SELECT 
        TRUE,
        ('boya yap işlem no ' || v_islemno || ' | ' || v_renk_ismi || ' (' || p_baz_kg || 'kg)')::TEXT,
        v_islemno;
        
EXCEPTION WHEN OTHERS THEN
    RETURN QUERY SELECT FALSE, (' HATA: ' || SQLERRM)::TEXT, NULL::INTEGER;
END;
$$;


--
-- Name: hazir_renkler(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.hazir_renkler() RETURNS TABLE(renk_kodu character varying, renk_ismi character varying, kartela character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT 
        h.renkkodu,
        h.renkismi,
        h.renkkartelasi
    FROM hazirrenk h
    ORDER BY h.renkismi;
END;
$$;


--
-- Name: hazne_listesi(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.hazne_listesi() RETURNS TABLE(hazne_no integer, pigment_isim character varying, pigment_marka character varying, kalan_gr integer, durum character varying, renk_kodu character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT 
        s.urunno,
        COALESCE(s.pigmentisim, 'BO┼Ş')::VARCHAR,
        COALESCE(s.pigmentmarka, '-')::VARCHAR,
        COALESCE(s.kalanpigmentgr, 0),
        CASE 
            WHEN s.kalanpigmentgr IS NULL OR s.kalanpigmentgr = 0 THEN 'T├£KEND─░'
            WHEN s.kalanpigmentgr < 500 THEN 'KR─░T─░K'
            WHEN s.kalanpigmentgr < 1500 THEN 'D├£┼Ş├£K'
            ELSE 'NORMAL'
        END::VARCHAR,
        CASE 
            WHEN s.kalanpigmentgr IS NULL OR s.kalanpigmentgr = 0 THEN 'RED'
            WHEN s.kalanpigmentgr < 500 THEN 'ORANGE'
            WHEN s.kalanpigmentgr < 1500 THEN 'YELLOW'
            ELSE 'GREEN'
        END::VARCHAR
    FROM stok s
    ORDER BY s.urunno;
END;
$$;


--
-- Name: log_bakim_trigger(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.log_bakim_trigger() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    -- INSERT ve UPDATE fark etmez, her bak─▒mda log al
    INSERT INTO makinelog (bakimturu, personelrolno)
    VALUES (NEW.bakimturu, NEW.personelrolno);

    RETURN NEW;
END;
$$;


--
-- Name: musteri_gecmis(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.musteri_gecmis() RETURNS TABLE(islemno integer, islemtarihi timestamp without time zone, musteriad character varying, musteriiletisim character varying, dukkanad character varying, personelad character varying, personelsoyad character varying, renkismi character varying, renkkodu character varying, bazkg integer, musterisoyad character varying, dukkantelno character varying, musteriadres character varying)
    LANGUAGE plpgsql
    AS $$
BEGIN
    RETURN QUERY
    SELECT
        k.islemno,
        k.islemtarihi,
        k.musteriad,
        k.musteriiletisim,
        k.dukkanad,
        k.personelad,
        k.personelsoyad,
        k.renkismi,
        k.renkkodu,
        k.bazkg,
        k.musterisoyad,
        k.dukkantelno,
        k.musteriadres
    FROM karisimkagidi k
    ORDER BY k.islemtarihi DESC;
END;
$$;


--
-- Name: stok_arttir(character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.stok_arttir(p_pigment character varying, p_miktar integer) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE stok
    SET kalanpigmentgr = kalanpigmentgr + p_miktar
    WHERE pigmentisim = p_pigment;
END;
$$;


--
-- Name: stok_azalt(character varying, integer); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.stok_azalt(p_pigment character varying, p_miktar integer) RETURNS void
    LANGUAGE plpgsql
    AS $$
BEGIN
    UPDATE stok
    SET kalanpigmentgr = kalanpigmentgr - p_miktar
    WHERE pigmentisim = p_pigment
      AND kalanpigmentgr >= p_miktar;
END;
$$;


--
-- Name: stok_azalt_trigger(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.stok_azalt_trigger() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
DECLARE
    v_pigment RECORD;
    v_stok_id INTEGER;
    v_renk_kodu VARCHAR;
BEGIN
    -- ─░┼şlem numaras─▒ndan renk kodunu al
    SELECT renkkodu INTO v_renk_kodu
    FROM karisimkagidi
    WHERE islemno = NEW.islemno;
    
    IF v_renk_kodu IS NULL THEN
        RAISE NOTICE 'UYARI: ─░┼şlem no % i├ğin renk kodu bulunamad─▒!', NEW.islemno;
        RETURN NEW;
    END IF;
    
    -- Bu renk i├ğin gereken t├╝m pigmentleri al
    FOR v_pigment IN 
        SELECT pigmentisim, pigmentmarka, pigment_miktar_gr
        FROM renk_pigment_detay
        WHERE renkkodu = v_renk_kodu
    LOOP
        -- Her pigment i├ğin stoktan d├╝┼ş
        UPDATE stok
        SET kalanpigmentgr = kalanpigmentgr - v_pigment.pigment_miktar_gr
        WHERE pigmentisim = v_pigment.pigmentisim
          AND pigmentmarka = v_pigment.pigmentmarka
          AND kalanpigmentgr >= v_pigment.pigment_miktar_gr
        RETURNING urunno INTO v_stok_id;
        
        -- Stok yetersizse uyar─▒ ver
        IF NOT FOUND THEN
            RAISE NOTICE 'UYARI: % - % pigmenti i├ğin yeterli stok yok! Gereken: % gr', 
                v_pigment.pigmentisim, v_pigment.pigmentmarka, v_pigment.pigment_miktar_gr;
        END IF;
        
        -- Stok hareketini kaydet (GE├ç─░C─░ OLARAK KALDIRILDI)
        -- INSERT INTO stok_hareket sat─▒r─▒ kald─▒r─▒ld─▒
        
    END LOOP;
    
    RETURN NEW;
END;
$$;


--
-- Name: stok_hareket_cikar_trg_fn(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.stok_hareket_cikar_trg_fn() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF NEW.tur = 'CIKAR' THEN
        PERFORM stok_azalt(NEW.pigmentisim, NEW.miktar);
    END IF;
    RETURN NEW;
END;
$$;


--
-- Name: stok_hareket_ekle_trg_fn(); Type: FUNCTION; Schema: public; Owner: -
--

CREATE FUNCTION public.stok_hareket_ekle_trg_fn() RETURNS trigger
    LANGUAGE plpgsql
    AS $$
BEGIN
    IF NEW.tur = 'EKLE' THEN
        PERFORM stok_arttir(NEW.pigmentisim, NEW.miktar);
    END IF;
    RETURN NEW;
END;
$$;


SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: bakimkaydi; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.bakimkaydi (
    bakimno integer NOT NULL,
    bakimtarihi date DEFAULT CURRENT_DATE NOT NULL,
    bakimturu character varying(100) NOT NULL,
    personelrolno integer NOT NULL
);


--
-- Name: bakimkaydi_bakimno_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.bakimkaydi_bakimno_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: bakimkaydi_bakimno_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.bakimkaydi_bakimno_seq OWNED BY public.bakimkaydi.bakimno;


--
-- Name: baz; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.baz (
    bazismi character varying(100) NOT NULL,
    firmaismi character varying(100) NOT NULL,
    kategori character varying(50) NOT NULL,
    bazyogunlugu numeric(10,3) NOT NULL,
    kg integer NOT NULL
);


--
-- Name: boyafirmasi; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.boyafirmasi (
    firmaismi character varying(100) NOT NULL,
    firmaadres character varying NOT NULL,
    firmailetisim character varying(100) NOT NULL,
    yetkiliismi character varying(100) NOT NULL,
    yetkiliitelno character varying(20) NOT NULL
);


--
-- Name: dukkan; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.dukkan (
    dukkanno integer NOT NULL,
    ad character varying(100) NOT NULL,
    adres character varying NOT NULL,
    telefonno character varying(20) NOT NULL
);


--
-- Name: dukkan_dukkanno_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.dukkan_dukkanno_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: dukkan_dukkanno_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.dukkan_dukkanno_seq OWNED BY public.dukkan.dukkanno;


--
-- Name: roller; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.roller (
    rolno integer NOT NULL,
    roladi character varying(50) NOT NULL,
    rolyetki character varying(50) NOT NULL,
    dukkanno integer
);


--
-- Name: global_role_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.global_role_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: global_role_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.global_role_seq OWNED BY public.roller.rolno;


--
-- Name: hazirrenk; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.hazirrenk (
    renkkodu character varying(50) NOT NULL,
    renkismi character varying(100) NOT NULL,
    renkkartelasi character varying(100)
);


--
-- Name: karisimkagidi; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.karisimkagidi (
    islemno integer NOT NULL,
    islemtarihi timestamp without time zone NOT NULL,
    musteriad character varying,
    musteriiletisim character varying,
    dukkanad character varying,
    personelad character varying,
    personelsoyad character varying,
    renkismi character varying,
    renkkodu character varying,
    bazkg integer,
    musterisoyad character varying,
    dukkantelno character varying,
    musteriadres character varying,
    musterirolno integer
);


--
-- Name: makinelog; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.makinelog (
    logno integer NOT NULL,
    logtarihi timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    bakimturu character varying(100) NOT NULL,
    personelrolno integer NOT NULL
);


--
-- Name: makinelog_logno_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.makinelog_logno_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: makinelog_logno_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.makinelog_logno_seq OWNED BY public.makinelog.logno;


--
-- Name: musteri; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.musteri (
    rolno integer NOT NULL,
    musteriad character varying(50) NOT NULL,
    musterisoyad character varying(50) NOT NULL,
    musteriiletisim character varying(100) NOT NULL,
    uyeliktarihi date DEFAULT CURRENT_DATE NOT NULL,
    musteriadres character varying NOT NULL
);


--
-- Name: personel; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.personel (
    rolno integer NOT NULL,
    personelad character varying(50) NOT NULL,
    personelsoyad character varying(50) NOT NULL,
    personeliletisim character varying(100) NOT NULL,
    personelrolno integer CONSTRAINT personel_personelno_not_null NOT NULL
);


--
-- Name: personel_personelno_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.personel_personelno_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: personel_personelno_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.personel_personelno_seq OWNED BY public.personel.personelrolno;


--
-- Name: pigment; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.pigment (
    pigmentisim character varying(100) NOT NULL,
    pigmentmarka character varying(100) NOT NULL,
    pigmentyogunluk numeric(10,3) NOT NULL
);


--
-- Name: renk_pigment_detay; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.renk_pigment_detay (
    detay_id integer NOT NULL,
    renkkodu character varying(50) NOT NULL,
    pigmentisim character varying(100) NOT NULL,
    pigmentmarka character varying(100) NOT NULL,
    pigment_miktar_gr integer NOT NULL
);


--
-- Name: renk_pigment_detay_detay_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.renk_pigment_detay_detay_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: renk_pigment_detay_detay_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.renk_pigment_detay_detay_id_seq OWNED BY public.renk_pigment_detay.detay_id;


--
-- Name: renkpigmentorani; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.renkpigmentorani (
    renkkodu character varying(50) NOT NULL,
    atilanpigmentgr integer NOT NULL,
    kackgbaz integer NOT NULL,
    pigmentyogunluk numeric(10,3) NOT NULL,
    bazyogunlugu numeric(10,3) NOT NULL
);


--
-- Name: stok; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.stok (
    urunno integer NOT NULL,
    kalanpigmentgr integer DEFAULT 0 NOT NULL,
    pigmentisim character varying(100) NOT NULL,
    pigmentmarka character varying(100) NOT NULL
);


--
-- Name: stok_hareket; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.stok_hareket (
    hareket_id integer NOT NULL,
    pigmentisim character varying(100) NOT NULL,
    pigmentmarka character varying(100) NOT NULL,
    miktar integer NOT NULL,
    tur character varying(10),
    tarih timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT stok_hareket_tur_check CHECK (((tur)::text = ANY (ARRAY[('EKLE'::character varying)::text, ('CIKAR'::character varying)::text])))
);


--
-- Name: stok_hareket_hareket_id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.stok_hareket_hareket_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: stok_hareket_hareket_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.stok_hareket_hareket_id_seq OWNED BY public.stok_hareket.hareket_id;


--
-- Name: stok_urunno_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.stok_urunno_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: stok_urunno_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.stok_urunno_seq OWNED BY public.stok.urunno;


--
-- Name: yapilanboya; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public.yapilanboya (
    islemno integer NOT NULL,
    kullanilanbazkg integer CONSTRAINT yapilanboya_kullanilanbazgr_not_null NOT NULL,
    kullanilanpigmentgr integer
);


--
-- Name: yapilanboya_islemno_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public.yapilanboya_islemno_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- Name: yapilanboya_islemno_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public.yapilanboya_islemno_seq OWNED BY public.yapilanboya.islemno;


--
-- Name: bakimkaydi bakimno; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.bakimkaydi ALTER COLUMN bakimno SET DEFAULT nextval('public.bakimkaydi_bakimno_seq'::regclass);


--
-- Name: dukkan dukkanno; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.dukkan ALTER COLUMN dukkanno SET DEFAULT nextval('public.dukkan_dukkanno_seq'::regclass);


--
-- Name: makinelog logno; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.makinelog ALTER COLUMN logno SET DEFAULT nextval('public.makinelog_logno_seq'::regclass);


--
-- Name: personel personelrolno; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.personel ALTER COLUMN personelrolno SET DEFAULT nextval('public.personel_personelno_seq'::regclass);


--
-- Name: renk_pigment_detay detay_id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.renk_pigment_detay ALTER COLUMN detay_id SET DEFAULT nextval('public.renk_pigment_detay_detay_id_seq'::regclass);


--
-- Name: roller rolno; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.roller ALTER COLUMN rolno SET DEFAULT nextval('public.global_role_seq'::regclass);


--
-- Name: stok urunno; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.stok ALTER COLUMN urunno SET DEFAULT nextval('public.stok_urunno_seq'::regclass);


--
-- Name: stok_hareket hareket_id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.stok_hareket ALTER COLUMN hareket_id SET DEFAULT nextval('public.stok_hareket_hareket_id_seq'::regclass);


--
-- Name: yapilanboya islemno; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.yapilanboya ALTER COLUMN islemno SET DEFAULT nextval('public.yapilanboya_islemno_seq'::regclass);


--
-- Data for Name: bakimkaydi; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.bakimkaydi (bakimno, bakimtarihi, bakimturu, personelrolno) VALUES (4, '2025-12-20', 'Boya Pompası', 19);


--
-- Data for Name: baz; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.baz (bazismi, firmaismi, kategori, bazyogunlugu, kg) VALUES ('İç Cephe Mat', 'Marshall Boya', 'İç Mekan', 1.450, 15);
INSERT INTO public.baz (bazismi, firmaismi, kategori, bazyogunlugu, kg) VALUES ('Dış Cephe Saten', 'Filli Boya', 'Dış Mekan', 1.520, 20);
INSERT INTO public.baz (bazismi, firmaismi, kategori, bazyogunlugu, kg) VALUES ('Ahşap Vernik', 'Düfa Boya', 'Ahşap', 0.980, 5);
INSERT INTO public.baz (bazismi, firmaismi, kategori, bazyogunlugu, kg) VALUES ('Metal Astar', 'Jotun Boya', 'Metal', 1.380, 10);
INSERT INTO public.baz (bazismi, firmaismi, kategori, bazyogunlugu, kg) VALUES ('Su Bazlı İpek', 'Dyo Boya', 'İç Mekan', 1.420, 15);
INSERT INTO public.baz (bazismi, firmaismi, kategori, bazyogunlugu, kg) VALUES ('Plastik Boya', 'Polisan Boya', 'İç Mekan', 1.390, 10);


--
-- Data for Name: boyafirmasi; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.boyafirmasi (firmaismi, firmaadres, firmailetisim, yetkiliismi, yetkiliitelno) VALUES ('Marshall Boya', 'İstanbul Sanayi Bölgesi No:45', 'info@marshall.com.tr', 'Ahmet Yılmaz', '0212-555-0101');
INSERT INTO public.boyafirmasi (firmaismi, firmaadres, firmailetisim, yetkiliismi, yetkiliitelno) VALUES ('Filli Boya', 'İzmir Kemalpaşa OSB 23. Sok', 'destek@filli.com.tr', 'Mehmet Kaya', '0232-555-0202');
INSERT INTO public.boyafirmasi (firmaismi, firmaadres, firmailetisim, yetkiliismi, yetkiliitelno) VALUES ('Düfa Boya', 'Ankara Sincan Sanayi', 'iletisim@dufa.com.tr', 'Ayşe Demir', '0312-555-0303');
INSERT INTO public.boyafirmasi (firmaismi, firmaadres, firmailetisim, yetkiliismi, yetkiliitelno) VALUES ('Jotun Boya', 'Bursa Nilüfer OSB', 'info@jotun.com.tr', 'Ali Şahin', '0224-555-0404');
INSERT INTO public.boyafirmasi (firmaismi, firmaadres, firmailetisim, yetkiliismi, yetkiliitelno) VALUES ('Dyo Boya', 'Gebze Kimya OSB', 'musteri@dyo.com.tr', 'Fatma Arslan', '0262-555-0505');
INSERT INTO public.boyafirmasi (firmaismi, firmaadres, firmailetisim, yetkiliismi, yetkiliitelno) VALUES ('Polisan Boya', 'Kocaeli Dilovası', 'info@polisan.com.tr', 'Zeynep Öztürk', '0262-555-0606');


--
-- Data for Name: dukkan; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.dukkan (dukkanno, ad, adres, telefonno) VALUES (1, 'sais', 'gumushane', '0535');
INSERT INTO public.dukkan (dukkanno, ad, adres, telefonno) VALUES (2, 'Yapı Market Kadıköy', 'Kadıköy Bahariye Cad. No:78', '0216-555-1001');
INSERT INTO public.dukkan (dukkanno, ad, adres, telefonno) VALUES (5, 'Mega Yapı İzmir', 'İzmir Alsancak Kordon 567', '0232-555-1004');
INSERT INTO public.dukkan (dukkanno, ad, adres, telefonno) VALUES (6, 'Homeplus Bursa', 'Bursa Nilüfer Özlüce Mah.', '0224-555-1005');
INSERT INTO public.dukkan (dukkanno, ad, adres, telefonno) VALUES (8, 'DIY Center Bakırköy', 'Bakırköy Ataköy 7-8-9-10 Mah.', '0212-555-1007');
INSERT INTO public.dukkan (dukkanno, ad, adres, telefonno) VALUES (23, 'deneme', 'sakarya', '0456');


--
-- Data for Name: hazirrenk; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.hazirrenk (renkkodu, renkismi, renkkartelasi) VALUES ('RAL-1013', 'İnci Beyazı', 'RAL Classic');
INSERT INTO public.hazirrenk (renkkodu, renkismi, renkkartelasi) VALUES ('RAL-3020', 'Trafik Kırmızısı', 'RAL Classic');
INSERT INTO public.hazirrenk (renkkodu, renkismi, renkkartelasi) VALUES ('RAL-5015', 'Gök Mavisi', 'RAL Classic');
INSERT INTO public.hazirrenk (renkkodu, renkismi, renkkartelasi) VALUES ('RAL-6018', 'Sarı Yeşil', 'RAL Classic');
INSERT INTO public.hazirrenk (renkkodu, renkismi, renkkartelasi) VALUES ('RAL-7035', 'Açık Gri', 'RAL Classic');
INSERT INTO public.hazirrenk (renkkodu, renkismi, renkkartelasi) VALUES ('RAL-9010', 'Saf Beyaz', 'RAL Classic');
INSERT INTO public.hazirrenk (renkkodu, renkismi, renkkartelasi) VALUES ('RAL-8014', 'Sepya Kahve', 'RAL Classic');
INSERT INTO public.hazirrenk (renkkodu, renkismi, renkkartelasi) VALUES ('RAL-4005', 'Mavi Lila', 'RAL Classic');


--
-- Data for Name: karisimkagidi; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- Data for Name: makinelog; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.makinelog (logno, logtarihi, bakimturu, personelrolno) VALUES (3, '2025-12-20 22:29:54.076286+03', 'Filtre Sistemi', 20);
INSERT INTO public.makinelog (logno, logtarihi, bakimturu, personelrolno) VALUES (4, '2025-12-20 22:32:01.90558+03', 'Boya Pompası', 19);


--
-- Data for Name: musteri; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- Data for Name: personel; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.personel (rolno, personelad, personelsoyad, personeliletisim, personelrolno) VALUES (1, 'Kemal', 'Arslan', 'kemal.arslan@boyaci.com', 1);
INSERT INTO public.personel (rolno, personelad, personelsoyad, personeliletisim, personelrolno) VALUES (5, 'Selin', 'Aydın', 'selin.aydin@boyaci.com', 5);
INSERT INTO public.personel (rolno, personelad, personelsoyad, personeliletisim, personelrolno) VALUES (16, 'samet', 'akan', '0500', 16);
INSERT INTO public.personel (rolno, personelad, personelsoyad, personeliletisim, personelrolno) VALUES (19, 'k', 'l', 'p', 19);


--
-- Data for Name: pigment; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.pigment (pigmentisim, pigmentmarka, pigmentyogunluk) VALUES ('Titanium White', 'Marshall Boya', 1.250);
INSERT INTO public.pigment (pigmentisim, pigmentmarka, pigmentyogunluk) VALUES ('Carbon Black', 'Marshall Boya', 1.180);
INSERT INTO public.pigment (pigmentisim, pigmentmarka, pigmentyogunluk) VALUES ('Iron Oxide Red', 'Filli Boya', 1.320);
INSERT INTO public.pigment (pigmentisim, pigmentmarka, pigmentyogunluk) VALUES ('Iron Oxide Yellow', 'Filli Boya', 1.290);
INSERT INTO public.pigment (pigmentisim, pigmentmarka, pigmentyogunluk) VALUES ('Chromium Oxide Green', 'Düfa Boya', 1.410);
INSERT INTO public.pigment (pigmentisim, pigmentmarka, pigmentyogunluk) VALUES ('Ultramarine Blue', 'Düfa Boya', 1.220);
INSERT INTO public.pigment (pigmentisim, pigmentmarka, pigmentyogunluk) VALUES ('Burnt Sienna', 'Jotun Boya', 1.270);
INSERT INTO public.pigment (pigmentisim, pigmentmarka, pigmentyogunluk) VALUES ('Raw Umber', 'Jotun Boya', 1.240);
INSERT INTO public.pigment (pigmentisim, pigmentmarka, pigmentyogunluk) VALUES ('Cobalt Blue', 'Dyo Boya', 1.350);
INSERT INTO public.pigment (pigmentisim, pigmentmarka, pigmentyogunluk) VALUES ('Cadmium Orange', 'Dyo Boya', 1.380);
INSERT INTO public.pigment (pigmentisim, pigmentmarka, pigmentyogunluk) VALUES ('Violet Oxide', 'Polisan Boya', 1.260);
INSERT INTO public.pigment (pigmentisim, pigmentmarka, pigmentyogunluk) VALUES ('Phthalo Green', 'Polisan Boya', 1.230);


--
-- Data for Name: renk_pigment_detay; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (1, 'RAL-1013', 'Titanium White', 'Marshall Boya', 120);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (2, 'RAL-1013', 'Iron Oxide Yellow', 'Filli Boya', 60);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (3, 'RAL-3020', 'Iron Oxide Red', 'Filli Boya', 200);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (4, 'RAL-3020', 'Titanium White', 'Marshall Boya', 50);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (5, 'RAL-5015', 'Ultramarine Blue', 'Düfa Boya', 150);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (6, 'RAL-5015', 'Cobalt Blue', 'Dyo Boya', 70);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (7, 'RAL-6018', 'Chromium Oxide Green', 'Düfa Boya', 130);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (8, 'RAL-6018', 'Iron Oxide Yellow', 'Filli Boya', 70);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (9, 'RAL-7035', 'Titanium White', 'Marshall Boya', 100);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (10, 'RAL-7035', 'Carbon Black', 'Marshall Boya', 50);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (11, 'RAL-9010', 'Titanium White', 'Marshall Boya', 80);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (12, 'RAL-8014', 'Burnt Sienna', 'Jotun Boya', 120);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (13, 'RAL-8014', 'Raw Umber', 'Jotun Boya', 70);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (14, 'RAL-4005', 'Violet Oxide', 'Polisan Boya', 140);
INSERT INTO public.renk_pigment_detay (detay_id, renkkodu, pigmentisim, pigmentmarka, pigment_miktar_gr) VALUES (15, 'RAL-4005', 'Ultramarine Blue', 'Düfa Boya', 70);


--
-- Data for Name: renkpigmentorani; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.renkpigmentorani (renkkodu, atilanpigmentgr, kackgbaz, pigmentyogunluk, bazyogunlugu) VALUES ('RAL-1013', 180, 10, 1.280, 1.450);
INSERT INTO public.renkpigmentorani (renkkodu, atilanpigmentgr, kackgbaz, pigmentyogunluk, bazyogunlugu) VALUES ('RAL-3020', 250, 10, 1.320, 1.450);
INSERT INTO public.renkpigmentorani (renkkodu, atilanpigmentgr, kackgbaz, pigmentyogunluk, bazyogunlugu) VALUES ('RAL-5015', 220, 10, 1.350, 1.450);
INSERT INTO public.renkpigmentorani (renkkodu, atilanpigmentgr, kackgbaz, pigmentyogunluk, bazyogunlugu) VALUES ('RAL-6018', 200, 10, 1.290, 1.450);
INSERT INTO public.renkpigmentorani (renkkodu, atilanpigmentgr, kackgbaz, pigmentyogunluk, bazyogunlugu) VALUES ('RAL-7035', 150, 10, 1.240, 1.450);
INSERT INTO public.renkpigmentorani (renkkodu, atilanpigmentgr, kackgbaz, pigmentyogunluk, bazyogunlugu) VALUES ('RAL-9010', 80, 10, 1.250, 1.450);
INSERT INTO public.renkpigmentorani (renkkodu, atilanpigmentgr, kackgbaz, pigmentyogunluk, bazyogunlugu) VALUES ('RAL-8014', 190, 10, 1.270, 1.450);
INSERT INTO public.renkpigmentorani (renkkodu, atilanpigmentgr, kackgbaz, pigmentyogunluk, bazyogunlugu) VALUES ('RAL-4005', 210, 10, 1.260, 1.450);


--
-- Data for Name: roller; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.roller (rolno, roladi, rolyetki, dukkanno) VALUES (13, 'Boya Ustası', 'BOYACI', 8);
INSERT INTO public.roller (rolno, roladi, rolyetki, dukkanno) VALUES (14, 'Müdür', 'YONETICI', 8);
INSERT INTO public.roller (rolno, roladi, rolyetki, dukkanno) VALUES (15, 'Boya Ustası', 'BOYACI', 5);
INSERT INTO public.roller (rolno, roladi, rolyetki, dukkanno) VALUES (16, 'Müdür', 'YONETICI', 5);
INSERT INTO public.roller (rolno, roladi, rolyetki, dukkanno) VALUES (17, 'Boya Ustası', 'BOYACI', 6);
INSERT INTO public.roller (rolno, roladi, rolyetki, dukkanno) VALUES (18, 'Müdür', 'YONETICI', 6);
INSERT INTO public.roller (rolno, roladi, rolyetki, dukkanno) VALUES (19, 'Boya Ustası', 'BOYACI', 23);
INSERT INTO public.roller (rolno, roladi, rolyetki, dukkanno) VALUES (20, 'Müdür', 'YONETICI', 23);
INSERT INTO public.roller (rolno, roladi, rolyetki, dukkanno) VALUES (1, 'Boya Ustası', 'BOYACI', 1);
INSERT INTO public.roller (rolno, roladi, rolyetki, dukkanno) VALUES (2, 'Boya Ustası', 'BOYACI', 2);
INSERT INTO public.roller (rolno, roladi, rolyetki, dukkanno) VALUES (4, 'Müdür', 'YONETICI', 1);
INSERT INTO public.roller (rolno, roladi, rolyetki, dukkanno) VALUES (5, 'Müdür', 'YONETICI', 2);


--
-- Data for Name: stok; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.stok (urunno, kalanpigmentgr, pigmentisim, pigmentmarka) VALUES (1, 1100, 'Titanium White', 'Marshall Boya');
INSERT INTO public.stok (urunno, kalanpigmentgr, pigmentisim, pigmentmarka) VALUES (7, 1500, 'Burnt Sienna', 'Jotun Boya');
INSERT INTO public.stok (urunno, kalanpigmentgr, pigmentisim, pigmentmarka) VALUES (9, 650, 'Cobalt Blue', 'Dyo Boya');
INSERT INTO public.stok (urunno, kalanpigmentgr, pigmentisim, pigmentmarka) VALUES (10, 1100, 'Cadmium Orange', 'Dyo Boya');
INSERT INTO public.stok (urunno, kalanpigmentgr, pigmentisim, pigmentmarka) VALUES (12, 1600, 'Phthalo Green', 'Polisan Boya');
INSERT INTO public.stok (urunno, kalanpigmentgr, pigmentisim, pigmentmarka) VALUES (11, 760, 'Violet Oxide', 'Polisan Boya');
INSERT INTO public.stok (urunno, kalanpigmentgr, pigmentisim, pigmentmarka) VALUES (8, 1200, 'Raw Umber', 'Jotun Boya');
INSERT INTO public.stok (urunno, kalanpigmentgr, pigmentisim, pigmentmarka) VALUES (4, 1000, 'Iron Oxide Yellow', 'Filli Boya');
INSERT INTO public.stok (urunno, kalanpigmentgr, pigmentisim, pigmentmarka) VALUES (5, 1000, 'Chromium Oxide Green', 'Düfa Boya');
INSERT INTO public.stok (urunno, kalanpigmentgr, pigmentisim, pigmentmarka) VALUES (6, 1000, 'Ultramarine Blue', 'Düfa Boya');
INSERT INTO public.stok (urunno, kalanpigmentgr, pigmentisim, pigmentmarka) VALUES (3, 800, 'Iron Oxide Red', 'Filli Boya');
INSERT INTO public.stok (urunno, kalanpigmentgr, pigmentisim, pigmentmarka) VALUES (2, 900, 'Carbon Black', 'Marshall Boya');


--
-- Data for Name: stok_hareket; Type: TABLE DATA; Schema: public; Owner: -
--

INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (1, 'Titanium White', 'Varsayılan', 1000, 'CIKAR', '2025-12-19 21:35:55.766656');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (2, 'Raw Umber', 'Varsayılan', 1000, 'CIKAR', '2025-12-19 21:36:07.485242');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (3, 'Titanium White', 'Varsayılan', 50, 'EKLE', '2025-12-19 21:36:19.403982');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (4, 'Titanium White', 'Varsayılan', 550, 'CIKAR', '2025-12-19 21:39:51.440523');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (5, 'Carbon Black', 'Varsayılan', 800, 'CIKAR', '2025-12-19 21:39:55.478678');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (6, 'Iron Oxide Red', 'Varsayılan', 550, 'EKLE', '2025-12-19 21:39:58.949107');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (7, 'Iron Oxide Yellow', 'Varsayılan', 200, 'CIKAR', '2025-12-19 21:40:02.363252');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (8, 'Chromium Oxide Green', 'Varsayılan', 200, 'EKLE', '2025-12-19 21:40:05.061322');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (9, 'Ultramarine Blue', 'Varsayılan', 720, 'EKLE', '2025-12-19 21:40:09.097122');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (10, 'Titanium White', 'Varsayılan', 50, 'EKLE', '2025-12-20 21:28:07.492638');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (11, 'Titanium White', 'Varsayılan', 50, 'CIKAR', '2025-12-20 21:28:14.789093');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (12, 'Titanium White', 'Varsayılan', 50, 'EKLE', '2025-12-20 21:42:41.792064');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (13, 'Titanium White', 'Varsayılan', 100, 'EKLE', '2025-12-20 21:43:36.111738');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (14, 'Titanium White', 'Varsayılan', 50, 'CIKAR', '2025-12-20 21:43:44.60809');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (15, 'Titanium White', 'Varsayılan', 50, 'EKLE', '2025-12-20 21:43:52.253539');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (16, 'Titanium White', 'Varsayılan', 50, 'CIKAR', '2025-12-20 21:45:17.287949');
INSERT INTO public.stok_hareket (hareket_id, pigmentisim, pigmentmarka, miktar, tur, tarih) VALUES (17, 'Titanium White', 'Varsayılan', 100, 'EKLE', '2025-12-20 21:45:23.184214');


--
-- Data for Name: yapilanboya; Type: TABLE DATA; Schema: public; Owner: -
--



--
-- Name: bakimkaydi_bakimno_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.bakimkaydi_bakimno_seq', 4, true);


--
-- Name: dukkan_dukkanno_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.dukkan_dukkanno_seq', 23, true);


--
-- Name: global_role_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.global_role_seq', 20, true);


--
-- Name: makinelog_logno_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.makinelog_logno_seq', 4, true);


--
-- Name: personel_personelno_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.personel_personelno_seq', 1, false);


--
-- Name: renk_pigment_detay_detay_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.renk_pigment_detay_detay_id_seq', 15, true);


--
-- Name: stok_hareket_hareket_id_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.stok_hareket_hareket_id_seq', 17, true);


--
-- Name: stok_urunno_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.stok_urunno_seq', 1, false);


--
-- Name: yapilanboya_islemno_seq; Type: SEQUENCE SET; Schema: public; Owner: -
--

SELECT pg_catalog.setval('public.yapilanboya_islemno_seq', 1, false);


--
-- Name: stok Pigment; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.stok
    ADD CONSTRAINT "Pigment" UNIQUE (pigmentisim, pigmentmarka);


--
-- Name: bakimkaydi bakimkaydi_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.bakimkaydi
    ADD CONSTRAINT bakimkaydi_pkey PRIMARY KEY (bakimno);


--
-- Name: baz baz_bazismi_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.baz
    ADD CONSTRAINT baz_bazismi_key UNIQUE (bazismi, kg, firmaismi);


--
-- Name: boyafirmasi boyafirmasi_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.boyafirmasi
    ADD CONSTRAINT boyafirmasi_pkey PRIMARY KEY (firmaismi);


--
-- Name: dukkan dukkan_ad_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.dukkan
    ADD CONSTRAINT dukkan_ad_key UNIQUE (ad);


--
-- Name: dukkan dukkan_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.dukkan
    ADD CONSTRAINT dukkan_pkey PRIMARY KEY (dukkanno);


--
-- Name: dukkan dukkan_telefonno_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.dukkan
    ADD CONSTRAINT dukkan_telefonno_key UNIQUE (telefonno);


--
-- Name: hazirrenk hazirrenk_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.hazirrenk
    ADD CONSTRAINT hazirrenk_pkey PRIMARY KEY (renkkodu);


--
-- Name: makinelog makinelog_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.makinelog
    ADD CONSTRAINT makinelog_pkey PRIMARY KEY (logno);


--
-- Name: musteri musteri_musteriad_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.musteri
    ADD CONSTRAINT musteri_musteriad_key UNIQUE (musteriad);


--
-- Name: musteri musteri_musteriadres_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.musteri
    ADD CONSTRAINT musteri_musteriadres_key UNIQUE (musteriadres);


--
-- Name: musteri musteri_musteriiletisim_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.musteri
    ADD CONSTRAINT musteri_musteriiletisim_key UNIQUE (musteriiletisim);


--
-- Name: musteri musteri_musterisoyad_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.musteri
    ADD CONSTRAINT musteri_musterisoyad_key UNIQUE (musterisoyad);


--
-- Name: musteri musteri_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.musteri
    ADD CONSTRAINT musteri_pkey PRIMARY KEY (rolno);


--
-- Name: musteri musteri_uniq_full; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.musteri
    ADD CONSTRAINT musteri_uniq_full UNIQUE (musteriad, musterisoyad, musteriiletisim);


--
-- Name: personel personel_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.personel
    ADD CONSTRAINT personel_pkey PRIMARY KEY (rolno);


--
-- Name: pigment pigment_pigmentyogunluk_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pigment
    ADD CONSTRAINT pigment_pigmentyogunluk_key UNIQUE (pigmentyogunluk);


--
-- Name: pigment pigment_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pigment
    ADD CONSTRAINT pigment_pkey PRIMARY KEY (pigmentisim, pigmentmarka);


--
-- Name: renk_pigment_detay renk_pigment_detay_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.renk_pigment_detay
    ADD CONSTRAINT renk_pigment_detay_pkey PRIMARY KEY (detay_id);


--
-- Name: renk_pigment_detay renk_pigment_detay_renkkodu_pigmentisim_pigmentmarka_key; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.renk_pigment_detay
    ADD CONSTRAINT renk_pigment_detay_renkkodu_pigmentisim_pigmentmarka_key UNIQUE (renkkodu, pigmentisim, pigmentmarka);


--
-- Name: renkpigmentorani renkpigmentorani_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.renkpigmentorani
    ADD CONSTRAINT renkpigmentorani_pkey PRIMARY KEY (renkkodu);


--
-- Name: roller roller_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.roller
    ADD CONSTRAINT roller_pkey PRIMARY KEY (rolno);


--
-- Name: stok_hareket stok_hareket_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.stok_hareket
    ADD CONSTRAINT stok_hareket_pkey PRIMARY KEY (hareket_id);


--
-- Name: stok stok_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.stok
    ADD CONSTRAINT stok_pkey PRIMARY KEY (urunno);


--
-- Name: karisimkagidi unique_karisimkagidi_islemno; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.karisimkagidi
    ADD CONSTRAINT unique_karisimkagidi_islemno PRIMARY KEY (islemno);


--
-- Name: yapilanboya yapilanboya_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.yapilanboya
    ADD CONSTRAINT yapilanboya_pkey PRIMARY KEY (islemno);


--
-- Name: idx_bakim_personel; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_bakim_personel ON public.bakimkaydi USING btree (personelrolno);


--
-- Name: idx_renk_pigment_pigment; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_renk_pigment_pigment ON public.renk_pigment_detay USING btree (pigmentisim, pigmentmarka);


--
-- Name: idx_renk_pigment_renk; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_renk_pigment_renk ON public.renk_pigment_detay USING btree (renkkodu);


--
-- Name: idx_stok_pigment; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX idx_stok_pigment ON public.stok USING btree (pigmentisim, pigmentmarka);


--
-- Name: index_atilanpigmentgr; Type: INDEX; Schema: public; Owner: -
--

CREATE INDEX index_atilanpigmentgr ON public.renkpigmentorani USING btree (atilanpigmentgr);


--
-- Name: stok_hareket stok_hareket_cikar_trigger; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER stok_hareket_cikar_trigger AFTER INSERT ON public.stok_hareket FOR EACH ROW EXECUTE FUNCTION public.stok_hareket_cikar_trg_fn();


--
-- Name: stok_hareket stok_hareket_ekle_trigger; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER stok_hareket_ekle_trigger AFTER INSERT ON public.stok_hareket FOR EACH ROW EXECUTE FUNCTION public.stok_hareket_ekle_trg_fn();


--
-- Name: bakimkaydi trigger_log_bakim; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trigger_log_bakim AFTER INSERT OR UPDATE ON public.bakimkaydi FOR EACH ROW EXECUTE FUNCTION public.log_bakim_trigger();


--
-- Name: yapilanboya trigger_stok_azalt; Type: TRIGGER; Schema: public; Owner: -
--

CREATE TRIGGER trigger_stok_azalt AFTER INSERT ON public.yapilanboya FOR EACH ROW EXECUTE FUNCTION public.stok_azalt_trigger();


--
-- Name: bakimkaydi fk_bakim_personel; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.bakimkaydi
    ADD CONSTRAINT fk_bakim_personel FOREIGN KEY (personelrolno) REFERENCES public.personel(rolno) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: baz fk_baz_firma; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.baz
    ADD CONSTRAINT fk_baz_firma FOREIGN KEY (firmaismi) REFERENCES public.boyafirmasi(firmaismi) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: karisimkagidi fk_dukkan_karisimkagidi_ad; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.karisimkagidi
    ADD CONSTRAINT fk_dukkan_karisimkagidi_ad FOREIGN KEY (dukkanad) REFERENCES public.dukkan(ad) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: hazirrenk fk_hazirrenk_renkorani; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.hazirrenk
    ADD CONSTRAINT fk_hazirrenk_renkorani FOREIGN KEY (renkkodu) REFERENCES public.renkpigmentorani(renkkodu) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: musteri fk_musteri_roller; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.musteri
    ADD CONSTRAINT fk_musteri_roller FOREIGN KEY (rolno) REFERENCES public.roller(rolno) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: personel fk_personel_roller; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.personel
    ADD CONSTRAINT fk_personel_roller FOREIGN KEY (rolno) REFERENCES public.roller(rolno) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: pigment fk_pigment_firma; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.pigment
    ADD CONSTRAINT fk_pigment_firma FOREIGN KEY (pigmentmarka) REFERENCES public.boyafirmasi(firmaismi) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: roller fk_roller_dukkan; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.roller
    ADD CONSTRAINT fk_roller_dukkan FOREIGN KEY (dukkanno) REFERENCES public.dukkan(dukkanno) ON DELETE CASCADE;


--
-- Name: stok fk_stok_pigment; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.stok
    ADD CONSTRAINT fk_stok_pigment FOREIGN KEY (pigmentisim, pigmentmarka) REFERENCES public.pigment(pigmentisim, pigmentmarka) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: karisimkagidi link_dukkan_karisimkagidi_tel; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.karisimkagidi
    ADD CONSTRAINT link_dukkan_karisimkagidi_tel FOREIGN KEY (dukkantelno) REFERENCES public.dukkan(telefonno) MATCH FULL ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: yapilanboya link_karisimkagidi_yapilanboya; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.yapilanboya
    ADD CONSTRAINT link_karisimkagidi_yapilanboya FOREIGN KEY (islemno) REFERENCES public.karisimkagidi(islemno) MATCH FULL ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: karisimkagidi link_musteri_karisimkagidi; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.karisimkagidi
    ADD CONSTRAINT link_musteri_karisimkagidi FOREIGN KEY (musterirolno) REFERENCES public.musteri(rolno) MATCH FULL ON UPDATE CASCADE ON DELETE RESTRICT;


--
-- Name: renk_pigment_detay renk_pigment_detay_pigmentisim_pigmentmarka_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.renk_pigment_detay
    ADD CONSTRAINT renk_pigment_detay_pigmentisim_pigmentmarka_fkey FOREIGN KEY (pigmentisim, pigmentmarka) REFERENCES public.pigment(pigmentisim, pigmentmarka) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- Name: renk_pigment_detay renk_pigment_detay_renkkodu_fkey; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public.renk_pigment_detay
    ADD CONSTRAINT renk_pigment_detay_renkkodu_fkey FOREIGN KEY (renkkodu) REFERENCES public.renkpigmentorani(renkkodu) ON UPDATE CASCADE ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict 0rJNHsoFr8LJ7g3NC3pwFn3gnpDNFDqzAJx4eBUK1wvICewPqjs1Wh7FXRvk0iT

