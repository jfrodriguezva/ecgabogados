"use client";
import { useEffect, useState, type FormEvent } from "react";
import { createCliente, getCasos, type Caso } from "@/lib/api";

export default function ClientesPage() {
  const [casos,setCasos]=useState<Caso[]>([]);
  const [mensaje,setMensaje]=useState("");
  useEffect(()=>{getCasos().then(setCasos)},[]);
  async function crear(e:FormEvent<HTMLFormElement>) {
    e.preventDefault(); setMensaje("");
    const f=new FormData(e.currentTarget);
    try {
      await createCliente({nombre:String(f.get("nombre")),email:String(f.get("email")),passwordTemporal:String(f.get("password")),casoId:Number(f.get("casoId"))});
      setMensaje("Cuenta creada y expediente vinculado. Comparte las credenciales temporalmente por un canal seguro.");
      e.currentTarget.reset();
    } catch { setMensaje("No fue posible crear la cuenta. Revisa si el correo ya está registrado."); }
  }
  return <section className="max-w-3xl">
    <p className="text-xs uppercase tracking-[0.22em] text-brand-gold">Administración</p>
    <h1 className="mt-2 font-display text-3xl font-bold">Alta de cliente</h1>
    <p className="mt-3 text-brand-creamSoft">Crea el acceso cuando una solicitud se convierta en cliente y vincúlalo con su primer expediente.</p>
    <form onSubmit={crear} className="mt-8 grid gap-4 border border-brand-line bg-brand-ink2 p-6">
      <input name="nombre" required placeholder="Nombre completo" className={field}/>
      <input name="email" required type="email" placeholder="Correo electrónico" className={field}/>
      <input name="password" required minLength={10} type="password" placeholder="Contraseña temporal (mínimo 10 caracteres)" className={field}/>
      <select name="casoId" required className={field}><option value="">Selecciona expediente</option>{casos.map(c=><option className="bg-brand-ink" key={c.id} value={c.id}>#{c.id} · {c.clienteNombre} · {c.tipo}</option>)}</select>
      <button className="bg-brand-gold px-5 py-3 text-xs font-semibold uppercase tracking-widest text-brand-ink">Crear acceso</button>
    </form>
    {mensaje&&<p className="mt-5 border border-brand-line p-4 text-sm">{mensaje}</p>}
  </section>
}
const field="border border-brand-line bg-brand-ink px-4 py-3 text-brand-cream outline-none focus:border-brand-gold";
