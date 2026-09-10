"use client";

import Link from "next/link";
import { useRouter } from "next/navigation";
import { useEffect, useState } from "react";
import Monogram from "@/components/Monogram";
import { descargarDocumentoCliente, getMisCasos, type MiCaso } from "@/lib/api";
import { deleteCookie, getCookie } from "@/lib/cookies";
import ConversacionCaso from "@/components/ConversacionCaso";
import CargaDocumentoCliente from "@/components/CargaDocumentoCliente";

const ETAPAS = ["Valoración", "Integración documental", "Preparación", "Presentado", "En trámite", "Resolución", "Concluido"];

export default function MiPortalPage() {
  const router = useRouter();
  const [casos, setCasos] = useState<MiCaso[]>([]);
  const [cargando, setCargando] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!getCookie("ecg_token")) { router.replace("/login"); return; }
    getMisCasos().then(setCasos).catch(() => setError("No fue posible cargar tus asuntos.")).finally(() => setCargando(false));
  }, [router]);

  function salir() {
    deleteCookie("ecg_token");
    deleteCookie("ecg_user");
    router.push("/");
  }

  return (
    <main className="min-h-screen bg-brand-ink text-brand-cream">
      <header className="border-b border-brand-line">
        <div className="mx-auto flex max-w-6xl items-center justify-between px-6 py-5">
          <Link href="/" className="flex items-center gap-3"><Monogram size={38}/><span className="font-display font-bold">ECG ABOGADOS</span></Link>
          <button onClick={salir} className="text-xs uppercase tracking-widest text-brand-gold">Cerrar sesión</button>
        </div>
      </header>
      <section className="mx-auto max-w-6xl px-6 py-12">
        <p className="text-xs uppercase tracking-[0.22em] text-brand-gold">Portal del cliente</p>
        <h1 className="mt-2 font-display text-4xl font-bold">Seguimiento de tus asuntos</h1>
        <p className="mt-3 max-w-2xl text-brand-creamSoft">Consulta etapas, documentos, próximas fechas y movimientos registrados por la abogada.</p>
        {cargando && <p className="mt-12 text-brand-creamSoft">Cargando información…</p>}
        {error && <p className="mt-8 border border-brand-gold p-4">{error}</p>}
        {!cargando && !error && casos.length === 0 && <p className="mt-10 border border-brand-line bg-brand-ink2 p-6">Tu cuenta aún no tiene asuntos vinculados. Comunícate con la abogada para solicitar la asociación.</p>}
        <div className="mt-10 space-y-8">
          {casos.map(caso => {
            const actual = Math.max(0, ETAPAS.indexOf(caso.etapa));
            return <article key={caso.id} className="border border-brand-line bg-brand-ink2 p-6 sm:p-8">
              <div className="flex flex-wrap items-start justify-between gap-4">
                <div><p className="text-xs uppercase tracking-widest text-brand-gold">Asunto #{caso.id}</p><h2 className="mt-2 font-display text-2xl font-bold">{caso.tipo}</h2></div>
                <span className="border border-brand-gold/50 px-3 py-1 text-xs uppercase tracking-widest">{caso.estatus}</span>
              </div>
              <div className="mt-7 grid grid-cols-2 gap-2 sm:grid-cols-7">
                {ETAPAS.map((etapa, i) => <div key={etapa} className={i <= actual ? "border-t-2 border-brand-gold pt-2 text-xs text-brand-cream" : "border-t border-brand-line pt-2 text-xs text-brand-creamSoft"}>{etapa}</div>)}
              </div>
              <div className="mt-8 grid gap-5 lg:grid-cols-3">
                <Panel titulo="Documentos"><p>{caso.documentos.length} archivo(s) disponible(s)</p>{caso.documentos.map(x => <button type="button" onClick={() => descargarDocumentoCliente(caso.id, x)} key={x.id} className="mt-2 block text-left text-brand-cream underline decoration-brand-gold/50 underline-offset-4 hover:text-brand-gold">{x.nombreArchivo}</button>)}</Panel>
                <Panel titulo="Fechas importantes">{caso.fechas.length ? caso.fechas.map(x => <p key={x.id} className="mt-2">{new Date(x.fechaLimite).toLocaleDateString("es-MX")} · {x.descripcion}</p>) : <p>Sin fechas publicadas.</p>}</Panel>
                <Panel titulo="Honorarios y pagos"><p>Total registrado: {caso.totalPagado.toLocaleString("es-MX", {style:"currency", currency:"MXN"})}</p><p className="mt-2 text-xs">Este portal no procesa pagos.</p></Panel>
              </div>
              <ConversacionCaso casoId={caso.id}/>
              <CargaDocumentoCliente casoId={caso.id}/>
            </article>;
          })}
        </div>
      </section>
    </main>
  );
}

function Panel({titulo, children}:{titulo:string; children:React.ReactNode}) {
  return <section className="border border-brand-line p-5"><h3 className="text-xs uppercase tracking-[0.18em] text-brand-gold">{titulo}</h3><div className="mt-3 text-sm text-brand-creamSoft">{children}</div></section>;
}
