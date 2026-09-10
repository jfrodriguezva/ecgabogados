"use client";
import { useEffect, useState, type FormEvent } from "react";
import { createTarifa, getTarifas, type Tarifa } from "@/lib/api";

export default function TarifasPage() {
  const [items,setItems]=useState<Tarifa[]>([]);
  const [error,setError]=useState("");
  const cargar=()=>getTarifas().then(setItems).catch(()=>setError("No fue posible cargar las tarifas."));
  useEffect(()=>{cargar()},[]);
  async function guardar(e:FormEvent<HTMLFormElement>) {
    e.preventDefault(); setError("");
    const f=new FormData(e.currentTarget);
    try {
      await createTarifa({area:String(f.get("area")),servicio:String(f.get("servicio")),concepto:String(f.get("concepto")),montoBase:Number(f.get("monto")),activa:true});
      e.currentTarget.reset(); await cargar();
    } catch { setError("No fue posible guardar la tarifa."); }
  }
  return <section>
    <p className="text-xs uppercase tracking-[0.22em] text-brand-gold">Configuración comercial</p>
    <h1 className="mt-2 font-display text-3xl font-bold">Tabla de tarifas</h1>
    <p className="mt-3 text-brand-creamSoft">Importes base para servicios jurídicos, fiscales y la futura comercializadora. Podrán ajustarse al contratar cada asunto.</p>
    <form onSubmit={guardar} className="mt-8 grid gap-3 border border-brand-line bg-brand-ink2 p-5 md:grid-cols-4">
      <select name="area" required className={field}><option>Jurídico</option><option>SAT</option><option>Comercializadora</option></select>
      <input name="servicio" required placeholder="Servicio" className={field}/>
      <input name="concepto" required placeholder="Concepto" className={field}/>
      <input name="monto" required min="0" step="0.01" type="number" placeholder="Monto base" className={field}/>
      <button className="bg-brand-gold px-4 py-3 text-xs font-semibold uppercase tracking-widest text-brand-ink md:col-span-4">Agregar tarifa</button>
    </form>
    {error&&<p className="mt-4 text-brand-gold">{error}</p>}
    <div className="mt-8 overflow-x-auto border border-brand-line">
      <table className="w-full text-left text-sm"><thead className="bg-brand-ink2 text-xs uppercase tracking-widest text-brand-gold"><tr><th className="p-4">Área</th><th className="p-4">Servicio</th><th className="p-4">Concepto</th><th className="p-4">Monto base</th></tr></thead>
      <tbody>{items.map(x=><tr key={x.id} className="border-t border-brand-line"><td className="p-4">{x.area}</td><td className="p-4">{x.servicio}</td><td className="p-4">{x.concepto}</td><td className="p-4">{x.montoBase.toLocaleString("es-MX",{style:"currency",currency:"MXN"})}</td></tr>)}</tbody></table>
    </div>
  </section>
}
const field="border border-brand-line bg-brand-ink px-4 py-3 text-sm text-brand-cream outline-none focus:border-brand-gold";
