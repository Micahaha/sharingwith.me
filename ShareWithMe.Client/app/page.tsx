// allows client-side code to be executed in the browser
"use client"

import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { useRef, useState, useEffect } from "react"
import { Plus, Download, Linkedin, Github, Copy, Check } from "lucide-react"


const API_URL = process.env.NEXT_PUBLIC_API_URL

export default function Page() {

  const fileInputRef = useRef<HTMLInputElement>(null)
  const [shareCode, setShareCode] = useState<string | null>(null)
  const [receiveCode, setReceiveCode] = useState("")
  const [uploadProgress, setUploadProgress] = useState<number>(0)
  const [expiresAt, setExpiresAt] = useState<Date | null>(null)
  const [timeRemaining, setTimeRemaining] = useState<string | null>(null)
  const [downloadUrl, setDownloadUrl] = useState<string | null>(null)
  const [originalFileName, setOriginalFileName] = useState<string | null>(null)
  const [copied, setCopied] = useState(false)


  useEffect(() => {
    if (!expiresAt) return
  
    const interval = setInterval(() => {
      const diff = expiresAt.getTime() - Date.now()
  
      if (diff <= 0) {
        setTimeRemaining("Expired")
        clearInterval(interval)
        return
      }
  
      const hours = Math.floor(diff / 1000 / 60 / 60)
      const minutes = Math.floor((diff / 1000 / 60) % 60)
      const seconds = Math.floor((diff / 1000) % 60)
  
      setTimeRemaining(
        `${hours}:${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`
      )
    }, 1000)
  
    return () => clearInterval(interval)
  }, [expiresAt])
  

  return (
    <div className="flex min-h-svh items-center justify-center bg-background px-4">
      <input type="file" hidden ref={fileInputRef}
      onChange={async (e) => {
        const file = e.target.files?.[0]
        if (!file) return

        const CHUNK_SIZE = 1024 * 1024 * 4 // 4MB
        const chunks: Blob[] = []

        // create chunks of 4MB each
        let offset = 0
        while (offset < file.size){
          chunks.push(file.slice(offset, offset + CHUNK_SIZE))
          offset += CHUNK_SIZE
        }


        const presignResponse = await fetch(`${API_URL}/api/files/presign`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            fileName: file.name,
            contentType: file.type,
            sizeBytes: file.size
          })
        })
        const { sasUrl, blobName } = await presignResponse.json()

        const blockIds: string[] = []
let uploaded = 0
const PARALLEL = 4

for (let i = 0; i < chunks.length; i += PARALLEL) {
  const batch = chunks.slice(i, i + PARALLEL)
  await Promise.all(batch.map(async (chunk, batchIndex) => {
    const chunkIndex = i + batchIndex
    const blockId = btoa(String(chunkIndex).padStart(6, '0'))
    blockIds[chunkIndex] = blockId

    await new Promise<void>((resolve, reject) => {
      const xhr = new XMLHttpRequest()
      xhr.onload = () => xhr.status < 400 ? resolve() : reject(new Error(`Block upload failed: ${xhr.status}`))
      xhr.onerror = () => reject(new Error("Network error"))
      xhr.open("PUT", `${sasUrl}&comp=block&blockid=${encodeURIComponent(blockId)}`)
      xhr.setRequestHeader("Content-Type", file.type)
      xhr.send(chunk)
    })

    uploaded += chunk.size
    setUploadProgress(Math.round((uploaded / file.size) * 100))
  }))
}

const xml = `<?xml version="1.0" encoding="utf-8"?><BlockList>${blockIds.map(id => `<Latest>${id}</Latest>`).join('')}</BlockList>`
await fetch(`${sasUrl}&comp=blocklist`, {
  method: "PUT",
  headers: { "Content-Type": "application/xml" },
  body: xml
})

        

        const registerRes = await fetch(`${API_URL}/api/files`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ blobName, originalFileName: file.name, contentType: file.type, sizeBytes: file.size })
        })
        const data = await registerRes.json()

        if (registerRes.ok) {
          setShareCode(data.url.split('/').pop())
          setExpiresAt(new Date(data.expiresAt))
          setUploadProgress(0)
        }
      }}
      />

      <div className="flex w-full max-w-sm flex-col gap-6">
        <div className="flex flex-col gap-1">
          <h1 className="text-2xl font-bold tracking-tight text-foreground text-center">ShareFilesWith.me</h1>
          <p className="text-sm text-muted-foreground">
            Quickly send or receive files with anyone using a simple key.
          </p>
        </div>

        <Card onClick={() => fileInputRef.current?.click()} className="cursor-pointer transition-shadow hover:shadow-md">
          <CardContent className="flex flex-col gap-2 p-6">
            <p className="font-semibold text-foreground text-center">Send</p>
            <div className="flex items-center justify-center py-6">
              <Plus className="h-10 w-10 text-primary" strokeWidth={1.5} />
            </div>
          </CardContent>
        </Card>
        {uploadProgress > 0 && uploadProgress < 100 && (
          <div className="w-full bg-muted rounded-full h-2">
            <div className="bg-primary h-2 rounded-full transition-all" style={{ width: `${uploadProgress}%` }} />
          </div>
        )}
        {shareCode && (
  <div className="flex items-center justify-center gap-2">
    <p className="text-center text-sm font-mono text-foreground">
      Your code: <span className="font-bold">{shareCode}</span>
    </p>
    <button
      onClick={() => {
        navigator.clipboard.writeText(shareCode)
        setCopied(true)
        setTimeout(() => setCopied(false), 2000)
      }}
      className="text-muted-foreground hover:text-foreground transition-colors"
      aria-label="Copy code"
    >
      {copied ? <Check className="h-4 w-4 text-green-500" /> : <Copy className="h-4 w-4" />}
    </button>
  </div>
)}
{timeRemaining && (
  <p className="text-center text-xs text-muted-foreground">
    Expires in: <span className={timeRemaining === "Expired" ? "text-destructive" : ""}>{timeRemaining}</span>
  </p>
)}

        <Card>
          <CardContent className="flex flex-col gap-2 p-6">
            <p className="font-semibold text-foreground text-center pb-5">Receive</p>
            <div className="relative">
              <Input
              value={receiveCode}
              onChange={(e) => setReceiveCode(e.target.value)}
              placeholder="Input key"
              className="pr-10"
/>
              <Download
                className="absolute right-3 top-1/2 h-4 w-4 -translate-y-1/2 text-muted-foreground cursor-pointer"
                onClick={async () => {
                  if (!receiveCode) return
                  const response = await fetch(`${API_URL}/api/files/${receiveCode}`)
                  const data = await response.json()
                  setDownloadUrl(data.sasUrl)
                  setOriginalFileName(data.originalFileName)
                  setExpiresAt(new Date(data.expiresAt))
                }}
              />
            </div>
            {downloadUrl && originalFileName && (
              <a href={downloadUrl} download={originalFileName} className="text-muted-foreground hover:text-foreground transition-colors text-sm font-mono text-foreground">
                {originalFileName} {expiresAt && (
                  <span className="text-xs text-muted-foreground">
                    Expires at: {expiresAt.toLocaleString()}
                  </span>
                )}
              </a>
            )}
          </CardContent>
        </Card>

        <div className="flex justify-center gap-2">
          <Button variant="ghost" size="icon" asChild>
            <a
              href="https://www.linkedin.com/in/micahahawinters/"
              target="_blank"
              rel="noopener noreferrer"
              aria-label="Micah Winters on LinkedIn"
            >
              <Linkedin className="h-5 w-5 text-muted-foreground" />
            </a>
          </Button>
          <Button variant="ghost" size="icon" asChild>
            <a
              href="https://github.com/Micahaha/sharingwith.me"
              target="_blank"
              rel="noopener noreferrer"
              aria-label="sharingwith.me on GitHub"
            >
              <Github className="h-5 w-5 text-muted-foreground" />
            </a>
          </Button>
        </div>
      </div>
    </div>
  )
}
