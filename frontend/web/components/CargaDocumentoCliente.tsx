"use client";
import { useState, type FormEvent } from "react";
import { subirDocumentoCliente } from "@/lib/api";
export default function CargaDocumentoCliente({casoId}:{casoId:number}) {
  const [estado,setEstado]=useState("");
  async function enviar(e:FormEvent<HTMLFormElement>) {
    e.preventDefault(); const form=e.currentTarget; const file=(new FormData(form).get("file") as File);
    if(!file?.size)return;
    setEstado("Subiendo…");
    try {await subirDocumentoCliente(casoId,file); setEstado("Documento enviado correctamente."); form.reset();}
    catch {setEstado("No fue posible subir el documento.");}
  }
  return <form onSubmit={enviar} className="mt-4 flex flex-wrap items-center gap-3"><input name="file" required type="file" accept=".pdf,.jpg,.jpeg,.png,.doc,.docx" className="min-w-0 text-xs text-brand-creamSoft"/><button className="border border-brand-gold px-4 py-2 text-xs uppercase text-brand-gold">Subir documento</button>{estado&&<span className="text-xs text-brand-creamSoft">{estado}</span>}</form>
}
