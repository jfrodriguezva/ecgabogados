import Link from "next/link";

export default function AvisoPrivacidadPage() {
  return (
    <main className="min-h-screen bg-brand-ink px-6 py-12 text-brand-cream">
      <article className="mx-auto max-w-3xl border border-brand-line bg-brand-ink2 p-7 sm:p-12">
        <p className="text-xs uppercase tracking-[0.22em] text-brand-gold">Documento editable · borrador</p>
        <h1 className="mt-3 font-display text-4xl font-bold">Aviso de privacidad integral</h1>
        <p className="mt-3 text-sm text-brand-creamSoft">Última actualización: 10 de septiembre de 2026.</p>
        <div className="mt-10 space-y-7 text-sm leading-7 text-brand-creamSoft">
          <Section title="1. Responsable">ECG Abogados, a cargo de la Lic. Erika Cruz García, es responsable del tratamiento de sus datos personales. Domicilio para oír y recibir notificaciones: <Placeholder>pendiente de definir</Placeholder>. Correo de contacto: erika.c.abogada@gmail.com.</Section>
          <Section title="2. Datos que podemos recabar">Datos de identificación y contacto; información familiar, jurídica, patrimonial, financiera o fiscal; documentos, mensajes y datos relacionados con los asuntos cuya valoración o atención se solicite. Algunos de estos datos podrían ser sensibles y solo deberán recabarse cuando sean necesarios.</Section>
          <Section title="3. Finalidades necesarias">Atender solicitudes de contacto y citas; evaluar, contratar y prestar servicios jurídicos o fiscales; integrar y administrar expedientes; comunicarnos con usted; recibir documentos; controlar honorarios y pagos; dar seguimiento a fechas y actuaciones; autenticar el acceso al portal y cumplir obligaciones legales.</Section>
          <Section title="4. Finalidades secundarias">Con autorización separada, podremos enviar información jurídica o fiscal, novedades y comunicaciones promocionales. Negarse a estas finalidades no afectará la prestación del servicio principal.</Section>
          <Section title="5. Transferencias">Los datos podrán comunicarse cuando sea necesario para el asunto contratado o por obligación legal a autoridades, órganos jurisdiccionales, peritos, notarios, proveedores tecnológicos u otros profesionales autorizados. Las transferencias que requieran consentimiento se identificarán antes de realizarse.</Section>
          <Section title="6. Derechos ARCO y revocación">Puede solicitar acceso, rectificación, cancelación u oposición, revocar su consentimiento o limitar el uso de sus datos escribiendo a erika.c.abogada@gmail.com. La solicitud deberá permitir verificar su identidad e indicar el derecho que desea ejercer. El procedimiento y sus plazos definitivos deberán incorporarse antes de publicar.</Section>
          <Section title="7. Seguridad, cookies y cambios">Aplicaremos medidas administrativas, técnicas y físicas razonables para proteger la información. El sitio podrá utilizar cookies indispensables para sesión y seguridad; cualquier analítica o publicidad requerirá la configuración de consentimiento correspondiente. Los cambios a este aviso se publicarán en esta misma página.</Section>
        </div>
        <aside className="mt-10 border-l-2 border-brand-gold bg-brand-gold/10 p-5 text-sm text-brand-cream">Este borrador contiene campos pendientes y debe ser revisado por la responsable antes de utilizarse para recabar información real, especialmente el domicilio para notificaciones, transferencias y procedimiento ARCO.</aside>
        <Link href="/" className="mt-8 inline-block text-xs uppercase tracking-widest text-brand-gold">← Volver al inicio</Link>
      </article>
    </main>
  );
}

function Section({ title, children }: { title: string; children: React.ReactNode }) {
  return <section><h2 className="font-display text-xl font-bold text-brand-cream">{title}</h2><p className="mt-2">{children}</p></section>;
}

function Placeholder({ children }: { children: React.ReactNode }) {
  return <strong className="text-brand-gold">[{children}]</strong>;
}
