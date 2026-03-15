// allows client-side code to be executed in the browser
"use client"

import { Card, CardContent } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Button } from "@/components/ui/button"
import { useRef, useState } from "react"
import { Plus, Download, Linkedin, Github } from "lucide-react"


export default function Page() {

  const fileInputRef = useRef<HTMLInputElement>(null)
  const [shareCode, setShareCode] = useState<string | null>(null)
  const [receiveCode, setReceiveCode] = useState("")
  const [uploadProgress, setUploadProgress] = useState<number>(0)



  return (
    <div className="flex min-h-svh items-center justify-center bg-background px-4">
      <input type="file" hidden ref={fileInputRef}
      onChange={async (e) => {
        const file = e.target.files?.[0]
        if (!file) return

        const presignResponse = await fetch("http://192.168.1.177:5038/api/files/presign", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            fileName: file.name,
            contentType: file.type,
            sizeBytes: file.size
          })
        })
        const { sasUrl, blobName } = await presignResponse.json()

        await new Promise<void>((resolve, reject) => {
          const xhr = new XMLHttpRequest()
          xhr.upload.onprogress = (ev) => {
            if (ev.lengthComputable) setUploadProgress(Math.round((ev.loaded / ev.total) * 100))
          }
          xhr.onload = () => xhr.status < 400 ? resolve() : reject(new Error(`Azure upload failed: ${xhr.status}`))
          xhr.onerror = () => reject(new Error("Network error"))
          xhr.open("PUT", sasUrl)
          xhr.setRequestHeader("x-ms-blob-type", "BlockBlob")
          xhr.setRequestHeader("Content-Type", file.type)
          xhr.send(file)
        })

        const registerRes = await fetch("http://192.168.1.177:5038/api/files", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ blobName, originalFileName: file.name, contentType: file.type, sizeBytes: file.size })
        })
        const data = await registerRes.json()

        if (registerRes.ok) {
          setShareCode(data.url.split('/').pop())
          setUploadProgress(0)
        }
      }}
      />

      <div className="flex w-full max-w-sm flex-col gap-6">
        <div className="flex flex-col gap-1">
          <h1 className="text-2xl font-bold tracking-tight text-foreground text-center">ShareWith.me</h1>
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
  <p className="text-center text-sm font-mono text-foreground">
    Your code: <span className="font-bold">{shareCode}</span>
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
                  const response = await fetch(`http://192.168.1.177:5038/api/files/${receiveCode}`)
                  if (!response.ok) return
                  const blob = await response.blob()
                  const url = URL.createObjectURL(blob)
                  const a = document.createElement("a")
                  a.href = url
                  a.download = receiveCode
                  a.click()
                  URL.revokeObjectURL(url)
                }}
              />
            </div>
          </CardContent>
        </Card>

        <div className="flex justify-center gap-2">
          <Button variant="ghost" size="icon" asChild>
            <a href="https://linkedin.com" target="_blank" rel="noopener noreferrer" aria-label="LinkedIn">
              <Linkedin className="h-5 w-5 text-muted-foreground" />
            </a>
          </Button>
          <Button variant="ghost" size="icon" asChild>
            <a href="https://github.com" target="_blank" rel="noopener noreferrer" aria-label="GitHub">
              <Github className="h-5 w-5 text-muted-foreground" />
            </a>
          </Button>
        </div>
      </div>
    </div>
  )
}
