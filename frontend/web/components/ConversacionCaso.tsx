"use client";
import { useCallback, useEffect, useState, type FormEvent } from "react";
import { enviarMensajeCaso, getMensajesCaso, type MensajeExpediente } from "@/lib/api";

export default function ConversacionCaso({casoId}:{casoId:number}) {
  const [items,setItems]=useState<MensajeExpediente[]>([]);
  const [texto,setTexto]=useState("");
  const [error,setError]=useState("");
  const cargar=useCallback(()=>getMensajesCaso(casoId).then(setItems).catch(()=>setError("No fue posible cargar la conversación.")),[casoId]);
  useEffect(()=>{void cargar()},[cargar]);
  async function enviar(e:FormEvent) {
    e.preventDefault(); if(!texto.trim()) return;
    try { await enviarMensajeCaso(casoId,texto); setTexto(""); await cargar(); } catch { setError("No fue posible enviar el mensaje."); }
  }
  return <section className="mt-5 border border-brand-line p-5">
    <h3 className="text-xs uppercase tracking-[0.18em] text-brand-gold">Conversación de este expediente</h3>
    <div className="mt-4 max-h-72 space-y-3 overflow-y-auto">
      {!items.length&&<p className="text-sm text-brand-creamSoft">Aún no hay mensajes.</p>}
      {items.map(x=><div key={x.id} className={x.autorRol==="Cliente"?"ml-8 border-l-2 border-brand-gold bg-brand-gold/10 p-3":"mr-8 border-l-2 border-brand-line bg-brand-ink p-3"}>
        <p className="text-xs text-brand-gold">{x.autorNombre} · {new Date(x.fechaEnvio).toLocaleString("es-MX")}</p>
        <p className="mt-1 text-sm">{x.mensaje}</p>
      </div>)}
    </div>
    <form onSubmit={enviar} className="mt-4 flex gap-2"><input value={texto} onChange={e=>setTexto(e.target.value)} maxLength={2000} placeholder="Escribe un mensaje sobre este asunto" className="min-w-0 flex-1 border border-brand-line bg-brand-ink px-4 py-3 text-sm outline-none focus:border-brand-gold"/><button className="bg-brand-gold px-5 text-xs font-semibold uppercase text-brand-ink">Enviar</button></form>
    {error&&<p className="mt-2 text-xs text-brand-gold">{error}</p>}
  </section>
}
